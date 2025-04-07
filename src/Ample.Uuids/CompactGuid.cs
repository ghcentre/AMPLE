namespace Ample.Uuids;

public static class CompactGuid
{
    /// <summary>
    /// Converts the compact string representation of a GUID to the equivalent <see cref="Guid"/> structure.
    /// </summary>
    /// <param name="stringValue">The string to convert.</param>
    /// <param name="result">When this method returns, contains the parsed value.
    /// If the method returns <see langword="true"/>, result contains a valid <see cref="Guid"/>.
    /// If the method returns <see langword="false"/>, result equals <see cref="Guid.Empty"/>.</param>
    /// <returns><see langword="true"/> if the parse operation was successful;
    /// otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(in ReadOnlySpan<char> stringValue, out Guid result)
    {
        result = Guid.Empty;

        if (stringValue.Length != CompactGuidConstants.CompactGuidStringLength)
        {
            return false;
        }

        Span<char> base64Chars = stackalloc char[CompactGuidConstants.Base64GuidStringLength];

        for (int i = 0; i < CompactGuidConstants.CompactGuidStringLength; i++)
        {
            base64Chars[i] = stringValue[i] switch
            {
                CompactGuidConstants.Chars.Minus => CompactGuidConstants.Chars.Plus,
                CompactGuidConstants.Chars.Underscore => CompactGuidConstants.Chars.Slash,
                _ => stringValue[i]
            };
        }

        base64Chars[CompactGuidConstants.CompactGuidStringLength] = CompactGuidConstants.Chars.Equal;
        base64Chars[CompactGuidConstants.CompactGuidStringLength + 1] = CompactGuidConstants.Chars.Equal;

        Span<byte> guidBytes = stackalloc byte[CompactGuidConstants.GuidByteArrayLength];
        bool converted = Convert.TryFromBase64Chars(base64Chars, guidBytes, out int bytesWritten);

        if (!converted || bytesWritten != CompactGuidConstants.GuidByteArrayLength)
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
    /// <returns>A structure that contains the value that was parsed.</returns>
    /// <exception cref="ArgumentNullException">Input is <see langword="null"/>.</exception>
    /// <exception cref="FormatException">Input is not in a recognized format.</exception>
    public static Guid Parse(ReadOnlySpan<char> stringValue)
    {
        if (TryParse(stringValue, out var guid))
        {
            return guid;
        }

        throw new FormatException();
    }
}
