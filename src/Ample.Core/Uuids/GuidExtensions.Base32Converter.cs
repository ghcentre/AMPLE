
namespace Ample.Core.Uuids;

public static partial class GuidExtensions
{
    private static class Base32GuidConverter
    {
        private static readonly char[] _crockfordAlphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ".ToCharArray(); // Crockford BASE32

        private const int _base32CharsInChunk = 8;
        private const int _bytesInChunk = 5;
        private const int _chunksInGuid = 3;
        private const int _base32CharsInRemainder = 2;
       
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
            for (int i = 0; i < 5; i++)
            {
                value |= (ulong)bytes[i] << (i * 8);
            }

            charsWritten = 0;

            for (int i = 7; i >= 0; i--)
            {
                int index = (int)(value % 32);
                result[i] = _crockfordAlphabet[index];

                value /= 32;
                charsWritten++;
            }

            return charsWritten == 8;
        }

        private static bool WriteFinalChars(byte byteValue, in Span<char> result, out int charsWritten)
        {
            charsWritten = 0;

            int value = byteValue;
            result[1] = _crockfordAlphabet[value % 32];
            charsWritten++;

            value /= 32;
            result[0] = _crockfordAlphabet[value];
            charsWritten++;

            return charsWritten == 2;
        }
    }
}
