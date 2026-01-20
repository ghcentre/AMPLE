namespace Ample.Streams;

internal static class Utils
{
    public static byte[] ThrowIfNullOrEmpty(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data is not { Length: > 0 })
        {
            throw new ArgumentException("Non-zero data buffer expected.", nameof(data));
        }

        return data;
    }
}
