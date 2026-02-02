
using Ample.Core.Uuids;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Ample.Streams.Tests.TestInfrastructure;

/// <summary>
/// Allows to read up to specified bytes, then blocks indefinitely.
/// </summary>
internal class NetworkStreamMock : Stream
{
    private readonly string _id = Guid.NewGuid().ToCompactString();
    private readonly long _capacity;
    private readonly bool _reportEndOfStream;
    private readonly ManualResetEvent _hasDataEvent;

    public NetworkStreamMock(long capacity, bool reportEndOfStream = false)
    {
        _capacity = capacity;
        _reportEndOfStream = reportEndOfStream;
        _hasDataEvent = new(capacity > 0);

        Debug.AutoFlush = true;

        Debug.WriteLine($"NetworkStreamMock..ctor({capacity}, {reportEndOfStream}) with id {_id}.");
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => _capacity;
    public override long Position { get; set; } = 0;

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        Debug.WriteLine($"NetworkStreamMock({_id}).Read(buffer[{buffer.Length}], {offset}, {count}). Position {Position}. Event {(_hasDataEvent.WaitOne(0) ? "set" : "reset")}.");

        if (_reportEndOfStream && !_hasDataEvent.WaitOne(0))
        {
            Debug.WriteLine($"NetworkStreamMock({_id}).Read(): returning 0 (EOS). Position: {Position}.");
            return 0;
        }

        Debug.WriteLine($"NetworkStreamMock({_id}).Read(): waiting.");
        _hasDataEvent.WaitOne();
        Debug.WriteLine($"NetworkStreamMock({_id}).Read(): done waiting.");

        int bytesRead = 0;

        if (Position == _capacity)
        {
            _hasDataEvent.Reset();
            if (_reportEndOfStream)
            {
                Debug.WriteLine($"NetworkStreamMock({_id}).Read(): returning 0 (EOS). Position: {Position}.");
                return 0;
            }
            
            Debug.WriteLine($"NetworkStreamMock({_id}).Read(): Position {Position} at capacity. Resetting event. Waiting forever (instead of returning {bytesRead}).");
            _hasDataEvent.WaitOne();

            return bytesRead;
        }

        for (int i = 0; i < count; i++)
        {
            if (Position >= _capacity)
            {
                Debug.WriteLine($"NetworkStreamMock({_id}).Read(): Position {Position} over capacity. Resetting event. Returning {bytesRead}.");
                _hasDataEvent.Reset();
                return bytesRead;
            }

            buffer[offset + i] = (byte)(Position % 256);

            Position++;
            bytesRead++;
        }

        Debug.WriteLine($"NetworkStreamMock({_id}).Read(): Event still set. Position {Position}. Returning {bytesRead}.");
        return bytesRead;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        Debug.WriteLine($"NetworkStreamMock({_id}).ReadAsync(buffer[{buffer.Length}], {offset}, {count}, ct).");

        var delayTask = Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        var readTask = Task.Factory.StartNew(() => Read(buffer, offset, count), cancellationToken);
        var firstCompletedTask = await Task.WhenAny(delayTask, readTask);
        
        cancellationToken.ThrowIfCancellationRequested();
        
        if (firstCompletedTask == delayTask)
        {
            Debug.WriteLine($"NetworkStreamMock({_id}).ReadAsync(): Task.Delay() completed. Returning 0.");
            return 0;
        }

        var result = await readTask;
        Debug.WriteLine($"NetworkStreamMock({_id}).ReadAsync(): Read() completed. Returning {result}.");
        return result; 
    }

    [SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "<Pending>")]
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        Debug.WriteLine($"NetworkStreamMock({_id}).ReadAsync(memory {buffer.Length}, ct).");

        var bytes = buffer.ToArray();
        int bytesRead = await ReadAsync(bytes, 0, bytes.Length, cancellationToken);
        MemoryExtensions.CopyTo(bytes, buffer);

        Debug.WriteLine($"NetworkStreamMock({_id}).ReadAsync(memory {buffer.Length}, ct) returning {bytesRead}.");
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
