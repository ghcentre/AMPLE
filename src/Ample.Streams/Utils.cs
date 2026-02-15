using System.Runtime.CompilerServices;

namespace Ample.Streams;

internal static class Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ThrowIfNullOrEmptyBuffer(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data is not { Length: > 0 })
        {
            throw new ArgumentException("Non-zero data buffer expected.", nameof(data));
        }

        return data;
    }
}
