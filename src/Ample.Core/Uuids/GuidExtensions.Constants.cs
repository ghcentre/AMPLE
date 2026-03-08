namespace Ample.Core.Uuids;

public partial class GuidExtensions
{
    private static class CompactGuidConstants
    {
        /// <summary>
        /// Represents the number of bytes in a GUID (Globally Unique Identifier) when expressed as a byte array.
        /// </summary>
        public const int GuidByteArrayLength = 16;

        /// <summary>
        /// Represents the length, in characters, of a compact (Base64-URL) string representation of a GUID.
        /// </summary>
        public const int CompactGuidStringLength = 22;
    }
}
