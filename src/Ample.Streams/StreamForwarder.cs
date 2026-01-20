using Ample.Streams.Abstractions;
using Ample.Streams.Exceptions;
using System.Threading;

namespace Ample.Streams;

public class StreamForwarder : IStreamForwarder
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly object _locker = new();

    private long _bytesTransfered = 0;

    public async Task<long> ForwardBidirectionalAsync(
        Stream clientStream,
        Stream serverStream,
        byte[] clientBuffer,
        byte[] serverBuffer,
        IInspector inspector,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(clientStream);
        ArgumentNullException.ThrowIfNull(serverStream);
        Utils.ThrowIfNullOrEmpty(clientBuffer);
        Utils.ThrowIfNullOrEmpty(serverBuffer);
        ArgumentNullException.ThrowIfNull(inspector);

        if (cancellationToken.IsCancellationRequested)
        {
            return 0;
        }

        if (!await _semaphore.WaitAsync(0, cancellationToken))
        {
            throw new ForwardingAlreadyRunningException();
        }

        try
        {
            _bytesTransfered = 0;
            if (cancellationToken.IsCancellationRequested)
            {
                return _bytesTransfered;
            }

            return await DoForwardingAsync(clientStream, serverStream, clientBuffer, serverBuffer, inspector, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<long> DoForwardingAsync(
        Stream clientStream,
        Stream serverStream,
        byte[] clientBuffer,
        byte[] serverBuffer,
        IInspector inspector,
        CancellationToken cancellationToken)
    {
        var clientChunk = new InspectionChunk(Direction.ClientToServer, clientBuffer);
        var clientState = new StreamState(clientChunk);

        var serverChunk = new InspectionChunk(Direction.ServerToClient, serverBuffer);
        var serverState = new StreamState(serverChunk);
        
        Task? clientToServerTask = null;
        Task? serverToClientTask = null;

        while (!clientState.EndOfStream || !serverState.EndOfStream)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return _bytesTransfered;
            }

            if (clientToServerTask == null || clientToServerTask.IsCompleted)
            {
                clientToServerTask = ThrowIfErrorAsync(
                    Operation.Read,
                    Side.Client,
                    async () => await ReadFromStreamAsync(clientStream, clientState, cancellationToken));
            }

            if (serverToClientTask == null || serverToClientTask.IsCompleted)
            {
                serverToClientTask = ThrowIfErrorAsync(
                    Operation.Read,
                    Side.Server,
                    async () => await ReadFromStreamAsync(serverStream, serverState, cancellationToken));
            }

            await Task.WhenAny(clientToServerTask, serverToClientTask);

            if (clientState.HasData)
            {
                var clientInspectionResult = await inspector.InspectAsync(clientState.Chunk);
                switch (clientInspectionResult)
                {
                    case InspectionResult.Continue:
                        await WriteToAnotherStreamAsync(
                            serverStream,
                            clientState.Chunk.Data,
                            clientState.Chunk.Length,
                            () => Interlocked.Add(ref _bytesTransfered, clientState.Chunk.Length),
                            cancellationToken);
                        clientState.ResetChunk();
                        break;

                    case InspectionResult.CollectMoreData:
                        break;

                    case InspectionResult.Discard:
                        clientState.ResetChunk();
                        break;
                }
            }

            if (serverState.HasData)
            {
                var serverInspectionResult = await inspector.InspectAsync(serverState.Chunk);
                switch (serverInspectionResult)
                {
                    case InspectionResult.Continue:
                        await WriteToAnotherStreamAsync(
                            clientStream,
                            serverState.Chunk.Data,
                            serverState.Chunk.Length,
                            () => Interlocked.Add(ref _bytesTransfered, serverState.Chunk.Length),
                            cancellationToken);
                        serverState.ResetChunk();
                        break;

                    case InspectionResult.CollectMoreData:
                        break;

                    case InspectionResult.Discard:
                        serverState.ResetChunk();
                        break;
                }
            }
        }

        return _bytesTransfered;
    }

    private static async Task ReadFromStreamAsync(Stream stream, StreamState streamState, CancellationToken cancellationToken)
    {
        var memory = streamState.Chunk.Data.AsMemory(streamState.Chunk.Length, streamState.Chunk.AvailableLength);
        int bytesRead = await stream.ReadAsync(memory, cancellationToken);

        if (bytesRead > 0)
        {
            streamState.IncrementBytesRead(bytesRead);
        }
        else if (bytesRead == 0)
        {
            streamState.EndOfStream = true;
        }
    }

    private async Task WriteToAnotherStreamAsync(
        Stream anotherStream,
        byte[] buffer,
        int count,
        Action incrementTransfered,
        CancellationToken cancellationToken)
    {
        var memory = buffer.AsMemory(0, count); // offset is always 0 -- writes entire not yet written buffer
        await anotherStream.WriteAsync(memory, cancellationToken);
        await anotherStream.FlushAsync(cancellationToken);
        incrementTransfered();
    }

    private async Task ThrowIfErrorAsync(Operation operation, Side side, Func<Task> func)
    {
        try
        {
            await func();
        }
        catch (Exception exception)
        {
            throw new ForwardException(operation, side, exception);
        }
    }
}
