namespace Ample.Core.Uuids;

public static partial class GuidExtensions
{
    /// <summary>
    /// Contains constants and <see langword="static"/> methods to convert GUID to its Crockford's BASE32 representation.
    /// </summary>
    private static class Base32GuidConverter
    {
        //
        // Crockford BASE32 alphabet
        // 
        private static readonly char[] _crockfordAlphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ".ToCharArray();
        private static readonly int _crockfordAlphabetLength = _crockfordAlphabet.Length;

        private const int _base32CharsInChunk = 8;          // number of BASE32 charachers in a 40-bit chunk
        private const int _bytesInChunk = 5;                // number of 8-bit bytes in a 40-bit chunk
        private const int _chunksInGuid = 3;                // number of chunks in a GUID
        private const int _base32CharsInRemainder = 2;      // numver of BASE32 charachers in a 'remainder'

        /// <summary>
        /// BASE32 GUID representation length
        /// </summary>       
        public const int GuidBase32CharLength = _base32CharsInChunk * _chunksInGuid + _base32CharsInRemainder;

        /// <summary>
        /// Writes the BASE32 representation of the GUID into the character buffer.
        /// </summary>
        /// <param name="guidBytes">Binary representation of the <see cref="Guid"/></param>
        /// <param name="resultChars">Buffer, of the type of <see cref="Span{Char}"/>, to write to.</param>
        /// <param name="charsWritten">When the method completes, contains number of the characters written.</param>
        /// <returns><see langword="true"/> if the number of characters written is equal to
        /// the BASE32 representation of the GUID (<see cref="GuidBase32CharLength"/>),
        /// or <see langword="false"/> otherwise.</returns>
        internal static bool WriteBase32Chars(in ReadOnlySpan<byte> guidBytes, in Span<char> resultChars, out int charsWritten)
        {
            charsWritten = 0;

            for (int i = 0; i < _chunksInGuid; i++)
            {
                ReadOnlySpan<byte> guidSlice = guidBytes.Slice(i * _bytesInChunk, _bytesInChunk);
                Span<char> resultSlice = resultChars.Slice(i * _base32CharsInChunk, _base32CharsInChunk);

                WriteBase32Chunk(guidSlice, resultSlice, out int chunkCharsWritten);
                charsWritten += chunkCharsWritten;
            }

            WriteFinalChars(
                guidBytes[_bytesInChunk * _chunksInGuid],
                resultChars.Slice(_base32CharsInChunk * _chunksInGuid, _base32CharsInRemainder),
                out int finalCharsWritten);
            charsWritten += finalCharsWritten;

            return charsWritten == GuidBase32CharLength;
        }

        //
        // Writes the 8-character BASE32 representation of the 5-byte GUID chunk.
        //
        private static bool WriteBase32Chunk(in ReadOnlySpan<byte> bytes, in Span<char> result, out int charsWritten)
        {
            ulong value = 0;
            for (int i = 0; i < _bytesInChunk; i++)
            {
                value |= (ulong)bytes[i] << (i * _base32CharsInChunk);
            }

            charsWritten = 0;

            for (int i = _base32CharsInChunk - 1; i >= 0; i--)
            {
                int index = (int)(value % (ulong)_crockfordAlphabetLength);
                result[i] = _crockfordAlphabet[index];

                value /= (ulong)_crockfordAlphabetLength;
                charsWritten++;
            }

            return charsWritten == _base32CharsInChunk;
        }

        // <summary>
        // Writes final 2 characters
        // </summary>
        private static bool WriteFinalChars(byte byteValue, in Span<char> result, out int charsWritten)
        {
            charsWritten = 0;

            int value = byteValue;
            result[1] = _crockfordAlphabet[value % _crockfordAlphabetLength];
            charsWritten++;

            value /= _crockfordAlphabetLength;
            result[0] = _crockfordAlphabet[value];
            charsWritten++;

            return charsWritten == _base32CharsInRemainder;
        }
    }
}
