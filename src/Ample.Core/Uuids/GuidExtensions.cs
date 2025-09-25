using System.Buffers.Text;
using System.Runtime.InteropServices;

namespace Ample.Core.Uuids;

public static class GuidExtensions
{
    /// <summary>
    /// Returns a compact string representation of the <see cref="Guid"/> instance.
    /// </summary>
    /// <param name="guid">The GUID instance.</param>
    /// <returns>The method returns a <c>compact</c> representation of the GUID instance.</returns>
    /// <remarks>
    /// <para>The <c>compact</c> representation of a GUID is a base-64 representation of the GUID bytes
    /// with <c>+</c> converted to <c>-</c>, <c>/</c> converted to <c>_</c> (base64 URI)
    /// and trailing <c>==</c> removed.</para>
    /// <para>Base64 URL representation is defined in the following RFCs:</para>
    /// <list type="bullet">
    ///     <item>(RFC 3548, par. 4).</item>
    ///     <item>(RFC 1575, appendix C)</item>
    /// </list>
    /// <para>See <see cref="CompactGuid.Parse(ReadOnlySpan{char})"/> to convert the compact GUID representation
    /// to the <see cref="Guid"/> value.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var guid = new Guid(Enumerable.Range(0, 16).Select(x => (byte)x).ToArray()); // {03020100-0504-0706-0809-0a0b0c0d0e0f}
    /// string compactString = guid.ToCompactString(); // AAECAwQFBgcICQoLDA0ODw
    /// </code>
    /// <seealso cref="CompactGuid.Parse(ReadOnlySpan{char})"></seealso>
    /// </example>
    public static string ToCompactString(this in Guid guid)
    {
        Span<byte> guidBytes = stackalloc byte[CompactGuidConstants.GuidByteArrayLength];
        MemoryMarshal.TryWrite(guidBytes, guid);

        Span<char> resultChars = stackalloc char[CompactGuidConstants.CompactGuidStringLength];
        Base64Url.TryEncodeToChars(guidBytes, resultChars, out _);

        return new string(resultChars);
    }

    public static string ToBase32String(this in Guid guid)
    {
        Span<byte> guidBytes = stackalloc byte[CompactGuidConstants.GuidByteArrayLength];
        MemoryMarshal.TryWrite(guidBytes, guid);

        Span<char> resultChars = stackalloc char[8 * 3 + 2];

        for (int i = 0; i < 3; i++)
        {
            Span<byte> guidSlice = guidBytes.Slice(i * 5, 5);
            Span<char> resultSlice = resultChars.Slice(i * 8, 8);
            WriteBase32Chunk(guidSlice, resultSlice, out _);
        }

        WriteFinalChars(guidBytes[15], resultChars.Slice(3 * 8, 2), out _);

        return new string(resultChars);
    }
    
    const string _alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ"; // Crockford BASE32

    private static bool WriteBase32Chunk(Span<byte> bytes, Span<char> result, out int charsWritten)
    {
        ulong value = 0;
        for (int i = 0; i < 5; i++)
        {
            value |= (ulong)bytes[i] << (i * 8);
        }
        
        charsWritten = 0;

        for (int i = 7; i >= 0; i--)
        {
            int index = (int)(value % 32);
            result[i] = _alphabet[index];

            value /= 32;
            charsWritten++;
        }

        return charsWritten == 8;
    }

    private static bool WriteFinalChars(byte byteValue, Span<char> result, out int charsWritten)
    {
        charsWritten = 0;

        int value = (int)byteValue;
        result[1] = _alphabet[value % 32];
        charsWritten++;

        value /= 32;
        result[0] = _alphabet[value];
        charsWritten++;

        return charsWritten == 2;
    }
}
