using Ample.Core.GuardClauses;
using Ample.Streams.Abstractions;
using Ample.Streams.Exceptions;

namespace Ample.Streams;

internal class StreamState(IInspectionChunk chunk)
{
    private bool _hasData = false;
    private readonly object _locker = new();

    public IInspectionChunk Chunk { get; } = Guard.Against.Null(chunk);

    public bool HasData
    {
        get
        {
            lock (_locker)
            {
                return _hasData;
            }
        }
    }

    public bool EndOfStream { get; set; } = false;

    public void IncrementBytesRead(int bytesRead)
    {
        lock (_locker)
        {
            try
            {
                Chunk.Length += bytesRead;
                _hasData = Chunk.Length > 0;
            }
            catch (ArgumentException excepton)
            {
                _hasData = true;
                throw new BufferOverflowException(excepton);
            }
        }
    }

    public void ResetChunk()
    {
        lock (_locker)
        {
            Chunk.Serial++;
            Chunk.Length = 0;
            _hasData = false;
        }
    }
}
