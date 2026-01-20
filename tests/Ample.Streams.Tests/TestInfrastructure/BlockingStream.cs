
namespace Ample.Streams.Tests.TestInfrastructure;

internal class BlockingStream(long initialSize) : Stream
{
    private readonly ManualResetEvent _hasDataEvent = new(true);

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => initialSize;
    public override long Position { get; set; } = 0;

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        _hasDataEvent.WaitOne();

        long i = 0;
        for (i = 0; i < count; i++)
        {
            if (Position >= initialSize)
            {
                _hasDataEvent.Reset();
                break;
            }

            buffer[offset++] = (byte)(Random.Shared.Next() % 256);
            Position++;
        }

        return (int)i;
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

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin != SeekOrigin.Begin)
        {
            throw new InvalidOperationException();
        }
        Position = offset;
        return Position;
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
    }
}
