using Ample.Core.GuardClauses;
using Ample.Streams.Abstractions;

namespace Ample.Streams;

public class InspectionChunk(Direction direction, byte[] data) : IInspectionChunk
{
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

    public byte[] Data { get; } = Utils.ThrowIfNullOrEmpty(data);
}
