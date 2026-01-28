
namespace Ample.Streams.Tests.TestInfrastructure;

/// <summary>
/// Allows to read up to <paramref name="initialSize"/> bytes, then blocks indefinitely.
/// </summary>
/// <param name="initialSize"></param>
internal class NetworkStreamMock(long capacity, bool reportEndOfStream = false) : Stream
{
    private readonly ManualResetEvent _hasDataEvent = new(capacity > 0);

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => capacity;
    public override long Position { get; set; } = 0;

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (reportEndOfStream && !_hasDataEvent.WaitOne(0))
        {
            return 0;
        }

        _hasDataEvent.WaitOne();

        int bytesRead = 0;

        for (int i = 0; i < count; i++)
        {
            if (Position >= capacity)
            {
                _hasDataEvent.Reset();
                return bytesRead;
            }

            buffer[offset + i] = (byte)(Position % 256);

            Position++;
            bytesRead++;
        }

        return bytesRead;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var delayTask = Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        var readTask = Task.Run(() => Read(buffer, offset, count), cancellationToken);
        var firstCompletedTask = await Task.WhenAny(delayTask, readTask);
        if (firstCompletedTask == delayTask)
        {
            return 0;
        }
        return readTask.Result;
    }
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var bytes = buffer.ToArray();
        int bytesRead = await ReadAsync(bytes, 0, bytes.Length, cancellationToken);
        MemoryExtensions.CopyTo(bytes, buffer);
        return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new InvalidOperationException();
    }

    public override void SetLength(long value)
    {
        throw new InvalidOperationException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
    }
}
