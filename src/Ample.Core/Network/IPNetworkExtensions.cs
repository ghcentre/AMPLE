using System.Buffers.Binary;
using System.Globalization;
using System.Net;

namespace Ample.Core.Network;

/// <summary>
/// Provides extension methods for working with <see cref="IPNetwork"/> and <see cref="IPAddress"/> types,
/// including utilities for parsing, address conversion, and network calculations.
/// </summary>
public static class IPNetworkExtensions
{
    extension(IPNetwork net)
    {
        /// <summary>
        /// Returns the network address for the specified <see cref="IPNetwork"/>.
        /// Currently, only IPv4 (<see cref="System.Net.Sockets.AddressFamily.InterNetwork"/>) networks are supported.
        /// </summary>
        /// <value>
        /// The <b>Base Address</b> for the given <see cref="IPNetwork"/>.
        /// In IPv4, this is the lowest address in the subnet.
        /// </value>
        public IPAddress NetworkAddress => net.BaseAddress.ForceIpV4();

        /// <summary>
        /// Returns the broadcast address for the given IPNetwork.
        /// Currently, only IPv4 (<see cref="System.Net.Sockets.AddressFamily.InterNetwork"/>) networks are supported.
        /// </summary>
        /// <value>
        /// The <c>Broadcast Address</c> for the given <see cref="IPNetwork"/>.
        /// In IPv4, this is the highest address in the subnet.
        /// This address is used to send packets to all hosts on the network.
        /// </value>
        public IPAddress BroadcastAddress
        {
            get
            {
                net.BaseAddress.ForceIpV4();
                uint baseAddress = net.BaseAddress.ToUint32();
                uint ipHost = HostToNetworkOrder(baseAddress | ~net.NetworkMask.ToUint32());
                return new IPAddress(ipHost);
            }
        }

        /// <summary>
        /// Gets the network mask for the given <see cref="IPNetwork"/>.
        /// Currently, only IPv4 (<see cref="System.Net.Sockets.AddressFamily.InterNetwork"/>) networks are supported.
        /// </summary>
        /// <value>
        /// The <c>Network Mask</c> for the given <see cref="IPNetwork"/> in an <see cref="IPAddress"/> form.
        /// </value>
        public IPAddress NetworkMask
        {
            get
            {
                net.BaseAddress.ForceIpV4();
                uint mask = PrefixLengthToUint32(net.PrefixLength);
                mask = HostToNetworkOrder(mask);
                return new IPAddress(mask);
            }
        }
    }

    extension(IPAddress address)
    {
        /// <summary>
        /// Converts the IPv4 address to its 32-bit unsigned integer representation.
        /// Throws if the address is not IPv4.
        /// </summary>
        /// <returns>The 32-bit unsigned integer representation of the IPv4 address.</returns>
        public uint ToUint32()
        {
            ArgumentNullException.ThrowIfNull(address);
            ForceIpV4(address);
            Span<byte> bytes = stackalloc byte[4];
            address.TryWriteBytes(bytes, out _);
            return (uint)(bytes[0] << 24) | (uint)(bytes[1] << 16) | (uint)(bytes[2] << 8) | bytes[3];
        }

        /// <summary>
        /// Ensures the <see cref="IPAddress"/> is an IPv4 address.
        /// Throws <see cref="InvalidOperationException"/> if not.
        /// </summary>
        /// <returns>The original IPv4 <see cref="IPAddress"/>.</returns>
        private IPAddress ForceIpV4()
        {
            ArgumentNullException.ThrowIfNull(address);
            if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                throw new FormatException($"InterNetwork address family expected for address: {address}");
            }
            return address;
        }
    }

    /// <summary>
    /// Parses a string representation of an IP network in CIDR notation (e.g., "192.168.1.0/24"),
    /// or in "address/mask" format (e.g., "192.168.1.0/255.255.255.0"),
    /// or a single IP address, returning an <see cref="IPNetwork"/> instance.
    /// </summary>
    /// <param name="source">The string to parse.</param>
    /// <returns>An <see cref="IPNetwork"/> representing the parsed network or host.</returns>
    /// <exception cref="ArgumentException">The source is null or empty.</exception>
    /// <exception cref="FormatException">The source is not in a valid format.</exception>"
    public static IPNetwork Parse(string source)
    {
        ArgumentException.ThrowIfNullOrEmpty(source);

        int slashPos = source.IndexOf('/');

        //
        // 192.168.42.43 => 192.168.42.43/32
        //
        if (slashPos == -1)
        {
            var singleHost = IPAddress.Parse(source);
            return new IPNetwork(singleHost, 32);
        }

        var host = source.AsSpan(..slashPos);
        var hostIP = IPAddress.Parse(host);
        ForceIpV4(hostIP);

        //
        // 192.168.0.2/255.255.255.0 => 192.168.0.0/24
        // 192.168.1.2/128.0.0.0 => skip (not a valid CIDR mask)
        //
        var networkMaskSpan = source.AsSpan((slashPos + 1)..);
        if (IPAddress.TryParse(networkMaskSpan, out var networkMask))
        {
            ForceIpV4(networkMask);
            if (TryConvertToCidrPrefix(networkMask, out int prefix))
            {
                return new IPNetwork(hostIP, prefix);
            }
        }

        // 192.168.3.2/24 => 192.168.3.0/24
        var cidr = source.AsSpan((slashPos + 1)..);
        int cidrInt = int.Parse(cidr, CultureInfo.InvariantCulture);

        var baseHost = ConvertToBaseAddress(hostIP, cidrInt);
        return new IPNetwork(baseHost, cidrInt);
    }

    private static bool TryConvertToCidrPrefix(IPAddress address, out int prefix)
    {
        ArgumentNullException.ThrowIfNull(address);

        uint mask = address.ToUint32();

        for (int i = 0; i < 32; i++)
        {
            uint candidate = PrefixLengthToUint32(i);
            if (mask == candidate)
            {
                prefix = i;
                return true;
            }
        }

        prefix = 0;
        return false;
    }

    /// <summary>
    /// Converts a prefix length (CIDR) to a 32-bit unsigned integer subnet mask.
    /// </summary>
    /// <param name="prefixLength">The prefix length (0-32).</param>
    /// <returns>The subnet mask as a 32-bit unsigned integer.</returns>
    private static uint PrefixLengthToUint32(int prefixLength) => prefixLength == 0 ? 0 : uint.MaxValue << (32 - prefixLength);

    /// <summary>
    /// Converts a 32-bit unsigned integer from host to network byte order.
    /// </summary>
    /// <param name="host">The value in host byte order.</param>
    /// <returns>The value in network byte order.</returns>
    private static uint HostToNetworkOrder(uint host) => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(host) : host;

    /// <summary>
    /// Calculates the base network address for a given IP address and prefix length.
    /// </summary>
    /// <param name="address">The IP address.</param>
    /// <param name="cidr">The prefix length.</param>
    /// <returns>The base network <see cref="IPAddress"/>.</returns>
    private static IPAddress ConvertToBaseAddress(IPAddress address, int cidr)
    {
        uint ip = address.ToUint32();
        uint mask = PrefixLengthToUint32(cidr);
        uint baseAddr = HostToNetworkOrder(ip & mask);
        return new IPAddress(baseAddr);
    }
}
