using Ample.Streams.Abstractions;
using Ample.Streams.Exceptions;

namespace Ample.Streams;

public class StreamForwarder : IStreamForwarder
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private long _clientToServerBytesTransferred = 0;
    private long _serverToClientBytesTransferred = 0;

    public long ClientToServerBytesTransferred => Interlocked.Read(ref _clientToServerBytesTransferred);

    public long ServerToClientBytesTransferred => Interlocked.Read(ref _serverToClientBytesTransferred);

    public async Task ForwardBidirectionalAsync(string sessionId,
                                                Stream clientStream,
                                                Stream serverStream,
                                                byte[] clientBuffer,
                                                byte[] serverBuffer,
                                                IInspector inspector,
                                                CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        ArgumentNullException.ThrowIfNull(clientStream);
        ArgumentNullException.ThrowIfNull(serverStream);
        Utils.ThrowIfNullOrEmptyBuffer(clientBuffer);
        Utils.ThrowIfNullOrEmptyBuffer(serverBuffer);
        ArgumentNullException.ThrowIfNull(inspector);

        Interlocked.Exchange(ref _clientToServerBytesTransferred, 0);
        Interlocked.Exchange(ref _serverToClientBytesTransferred, 0);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (!await _semaphore.WaitAsync(0, cancellationToken))
        {
            throw new ForwardingAlreadyRunningException();
        }

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await DoForwardingAsync(
                sessionId,
                clientStream,
                serverStream,
                clientBuffer,
                serverBuffer,
                inspector,
                cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task DoForwardingAsync(string sessionId,
                                         Stream clientStream,
                                         Stream serverStream,
                                         byte[] clientBuffer,
                                         byte[] serverBuffer,
                                         IInspector inspector,
                                         CancellationToken cancellationToken)
    {
        var clientChunk = new InspectionChunk(sessionId, Direction.ClientToServer, clientBuffer);
        var clientState = new StreamState(clientChunk);

        var serverChunk = new InspectionChunk(sessionId, Direction.ServerToClient, serverBuffer);
        var serverState = new StreamState(serverChunk);

        Task? clientToServerTask = null;
        Task? serverToClientTask = null;

        do
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
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
                await HandleStreamStateData(
                    inspector,
                    clientState,
                    Side.Client,
                    clientStream,
                    serverStream,
                    x => Interlocked.Add(ref _serverToClientBytesTransferred, x),
                    x => Interlocked.Add(ref _clientToServerBytesTransferred, x),
                    cancellationToken);
            }

            if (serverState.HasData)
            {
                await HandleStreamStateData(
                    inspector,
                    serverState,
                    Side.Server,
                    serverStream,
                    clientStream,
                    x => Interlocked.Add(ref _clientToServerBytesTransferred, x),
                    x => Interlocked.Add(ref _serverToClientBytesTransferred, x),
                    cancellationToken);
            }
        } while (!clientState.EndOfStream && !serverState.EndOfStream);
    }

    private static async Task HandleStreamStateData(IInspector inspector,
                                                    StreamState state,
                                                    Side side,
                                                    Stream associatedStream,
                                                    Stream anotherStream,
                                                    Action<int> associatedStreamTransferredBytesIncrement,
                                                    Action<int> anotherStreamTransferredBytesIncrement,
                                                    CancellationToken cancellationToken)
    {
        var serverInspectionResult = await inspector.InspectAsync(state.Chunk);

        switch (serverInspectionResult)
        {
            case InspectionResult.Send:
                await ThrowIfErrorAsync(
                    Operation.Write,
                    side,
                    async () => await WriteToAnotherStreamAsync(
                        anotherStream,
                        state.Chunk.Data,
                        state.Chunk.Length,
                        () => anotherStreamTransferredBytesIncrement(state.Chunk.Length),
                        cancellationToken));
                state.ResetChunk();
                break;

            case InspectionResult.CollectMoreData:
                break;

            case InspectionResult.Discard:
                state.ResetChunk();
                break;

            case InspectionResult.Reply:
                await ThrowIfErrorAsync(
                    Operation.Reply,
                    Side.Server,
                    async () => await WriteToAnotherStreamAsync(
                        associatedStream,
                        state.Chunk.Data,
                        state.Chunk.Length,
                        () => associatedStreamTransferredBytesIncrement(state.Chunk.Length),
                        cancellationToken));
                state.ResetChunk();
                break;
        }
    }

    private static async Task ReadFromStreamAsync(Stream stream, StreamState streamState, CancellationToken cancellationToken)
    {
        var memory = streamState.Chunk.Data.AsMemory(streamState.Chunk.Length, streamState.Chunk.AvailableLength);
        int bytesRead = await stream.ReadAsync(memory, cancellationToken).AsTask();

        //int bytesRead = await ReadFromStreamExperimentalAsync(stream, streamState.Chunk.Data, streamState.Chunk.Length, streamState.Chunk.AvailableLength, cancellationToken);

        if (bytesRead > 0)
        {
            streamState.IncrementBytesRead(bytesRead);
        }
        else if (bytesRead == 0)
        {
            streamState.EndOfStream = true;
        }
    }

    private static async Task WriteToAnotherStreamAsync(
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

    private static async Task ThrowIfErrorAsync(Operation operation, Side side, Func<Task> func)
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
