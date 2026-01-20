namespace Ample.Streams;

/// <summary>
/// Result of inspection perfomed by <see cref="IStreamInspector"/>.
/// </summary>
public enum InspectionResult
{
    /// <summary>
    /// Send the inspected data to another connected party.
    /// </summary>
    Continue = 0,

    /// <summary>
    /// Collect more data before making a decision.
    /// </summary>
    CollectMoreData = 1,

    /// <summary>
    /// Discard the inspected data.
    /// </summary>
    Discard = 2,
}
