namespace Ample.Streams.Abstractions;

public interface IInspectionChunk
{
    // <summary>
    /// Data flow direction.
    /// </summary>
    Direction Direction { get; }

    /// <summary>
    /// Gets or sets the serial number associated with the inspection chunk.
    /// </summary>
    ulong Serial { get; set; }

    /// <summary>
    /// Data chunk length.
    /// </summary>
    int Length { get; set; }

    /// <summary>
    /// Available length in the data chunk.
    /// </summary>
    public int AvailableLength => Data.Length - Length;

    /// <summary>
    /// Data buffer. The length of valid data is indicated by the <see cref="Length"/> property.
    /// </summary>
    byte[] Data { get; }
}
