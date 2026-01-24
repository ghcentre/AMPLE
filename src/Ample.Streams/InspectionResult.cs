namespace Ample.Streams;

/// <summary>
/// Result of inspection perfomed by <see cref="Abstractions.IInspector"/>.
/// </summary>
public enum InspectionResult
{
    /// <summary>
    /// Send the inspected data to another connected party.
    /// Discard the data after sending.
    /// </summary>
    Send = 0,

    /// <summary>
    /// Do not send and do not discard the inspected data yet.
    /// </summary>
    CollectMoreData = 1,

    /// <summary>
    /// Discard the inspected data.
    /// </summary>
    Discard = 2,

    /// <summary>
    /// Reply with the inspected data, as if the data came from another connected party.
    /// Discard the data after replying.
    /// </summary>
    Reply = 3,
}
