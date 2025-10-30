namespace Ample.Core.Uuids;

/// <summary>
/// Constants used by the <see cref="CompactGuid"/> and <see cref="GuidExtensions"/> classes.
/// </summary>
internal static class CompactGuidConstants
{
    /// <summary>
    /// Represents the number of bytes in a GUID (Globally Unique Identifier) when expressed as a byte array.
    /// </summary>
    internal const int GuidByteArrayLength = 16;

    /// <summary>
    /// Represents the length, in characters, of a compact (Base64-URL) string representation of a GUID.
    /// </summary>
    internal const int CompactGuidStringLength = 22;
}