namespace Ample.Uuids;

internal static class CompactGuidConstants
{
    public const int GuidByteArrayLength = 16;

    public const int CompactGuidStringLength = 22;

    public const int Base64GuidStringLength = CompactGuidStringLength + 2;

    public static class Chars
    {
        public const char Equal = '=';
        public const char Minus = '-';
        public const char Plus = '+';
        public const char Underscore = '_';
        public const char Slash = '/';
    }

    public static class Bytes
    {
        public const byte Plus = (byte)Chars.Plus;
        public const byte Slash = (byte)Chars.Slash;
    }
}
