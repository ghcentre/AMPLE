using Ample.Core.GuardClauses;
using Ample.Streams.Abstractions;

namespace Ample.Streams;

public class InspectionChunk(string sessionId, Direction direction, byte[] data) : IInspectionChunk
{
    public string SessionId { get; } = Guard.Against.NullOrWhiteSpace(sessionId);
    
    public Direction Direction { get; } = direction;

    public ulong Serial { get; set; }

    public int Length
    {
        get => field;
        set
        {
            Guard.Against.Negative(value);
            Guard.Against.GreaterThan(value, Data.Length);
            field = value;
        }
    }

    public byte[] Data { get; } = Utils.ThrowIfNullOrEmptyBuffer(data);
}
