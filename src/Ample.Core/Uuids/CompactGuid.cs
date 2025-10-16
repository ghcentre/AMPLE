using System.Buffers.Text;

namespace Ample.Core.Uuids;

/// <summary>
/// Contains <see langword="static"/> methods for converting the compact string representation of a GUID
/// to the equivalent <see cref="Guid"/> structure.
/// </summary>
public static class CompactGuid
{
    /// <summary>
    /// Converts the compact string representation of a GUID to the equivalent <see cref="Guid"/> structure.
    /// </summary>
    /// <param name="source">The string to convert.</param>
    /// <param name="result">When this method returns, contains the parsed value.
    /// If the method returns <see langword="true"/>, result contains a valid <see cref="Guid"/>.
    /// If the method returns <see langword="false"/>, result equals <see cref="Guid.Empty"/>.</param>
    /// <returns><see langword="true"/> if the parse operation was successful;
    /// otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(in ReadOnlySpan<char> source, out Guid result)
    {
        result = Guid.Empty;

        if (source.Length != CompactGuidConstants.CompactGuidStringLength || !Base64Url.IsValid(source))
        {
            return false;
        }

        Span<byte> guidBytes = stackalloc byte[CompactGuidConstants.GuidByteArrayLength];

        bool success = Base64Url.TryDecodeFromChars(source, guidBytes, out int bytesWritten);
        if (!success || bytesWritten != CompactGuidConstants.GuidByteArrayLength)
        {
            return false;
        }

        result = new Guid(guidBytes);
        return true;
    }

    /// <summary>
    /// Converts the compact string representation of a GUID to the equivalent <see cref="Guid"/> structure.
    /// </summary>
    /// <param name="stringValue">The string to convert.</param>
    /// <returns>A <see cref="Guid"/> structure that contains the value that was parsed.</returns>
    /// <exception cref="FormatException">Input is <see langword="null"/> not in a recognized format.</exception>
    public static Guid Parse(in ReadOnlySpan<char> stringValue)
    {
        if (TryParse(stringValue, out var guid))
        {
            return guid;
        }

        throw new FormatException(SR.Format_InvalidString);
    }
}