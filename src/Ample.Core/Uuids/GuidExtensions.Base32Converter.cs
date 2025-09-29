
namespace Ample.Core.Uuids;

public static partial class GuidExtensions
{
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
       
        //
        // BASE32 GUID representation length
        //
        public const int GuidBase32CharLength = _base32CharsInChunk * _chunksInGuid + _base32CharsInRemainder;

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
