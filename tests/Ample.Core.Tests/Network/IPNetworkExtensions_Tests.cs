using Ample.Core.Network;
using System.Net;

namespace Ample.Core.Tests.Network;

public class IPNetworkExtensions_Tests
{
    #region NetworkAddress
    
    [Fact]
    public void NetworkAddress_V4_Equals_BaseAddress()
    {
        var sut = new IPNetwork(IPAddress.Parse("192.168.1.1"), 24);
        var networkAddress = sut.NetworkAddress;
        networkAddress.ShouldBe(IPAddress.Parse("192.168.1.0"));
    }

    [Fact]
    public void NetworkAddress_V6_Throws()
    {
        var sut = new IPNetwork(IPAddress.Parse("2002:0bcf:2002::0"), 24);
        Should.Throw<FormatException>(() => sut.NetworkAddress);
    }

    #endregion

    #region NetworkMask
    
    [Fact]
    public void NetworkMask_V4_CorrectMask()
    {
        var sut = IPNetwork.Parse("192.168.0.0/24");
        var actual = sut.NetworkMask;
        var expected = IPAddress.Parse("255.255.255.0");
        actual.ShouldBe(expected);
    }

    #endregion

    #region IPAddress.ToUint32

    [Fact]
    public void ToUint_V4_ProducesCorrectResult()
    {
        var sut = IPAddress.Parse("192.168.1.24");
        var result = sut.ToUint32();
        result.ShouldBe(0xc0a80118);
    }

    [Fact]
    public void ToUint_V6_Throws()
    {
        var sut = IPAddress.Parse("2002:bf00::1");
        Should.Throw<FormatException>(() => sut.ToUint32());
    }

    #endregion

    #region BroadcastAddress
    
    [Theory]
    [InlineData("192.168.0.0/24", "192.168.0.255")]
    [InlineData("10.0.0.0/8", "10.255.255.255")]
    public void BroadcastAddress_V4_ProducesCorrectResult(string network, string broadcast)
    {
        var sut = IPNetwork.Parse(network);
        var actual = sut.BroadcastAddress;
        var expected = IPAddress.Parse(broadcast);
        actual.ShouldBe(expected);
    }

    #endregion

    #region NetworkMask
    
    [Fact]
    public void NetworkMask_V4_ProducesCorrectMask()
    {
        var sut = IPNetwork.Parse("192.168.111.0/24");
        var expected = IPAddress.Parse("255.255.255.0");
        var actual = sut.NetworkMask;
        actual.ShouldBe(expected);
    }

    #endregion

    #region IPNetworkExtensions.Parse

    [Theory]
    [InlineData("192.168.44.2/24",              "192.168.44.0", 24)]
    [InlineData("192.168.55.2/255.255.255.0",   "192.168.55.0", 24)]
    [InlineData("230.2.3.4/128.0.0.0",          "128.0.0.0",    1)]
    [InlineData("10.0.1.2/8",                   "10.0.0.0",     8)]
    [InlineData("1.2.3.4/0",                    "0.0.0.0",      0)]
    [InlineData("5.6.7.8/0.0.0.0",              "0.0.0.0",      0)]
    public void ParseAny_Parses(string network, string baseAddress, int prefixLength)
    {
        var sut = IPNetworkExtensions.Parse(network);
        var expectedBaseAddress = IPAddress.Parse(baseAddress);
        sut.BaseAddress.ShouldBe(expectedBaseAddress);
        sut.PrefixLength.ShouldBe(prefixLength);
    }

    [Fact]
    public void Parse_Invalid_Throws()
    {
        Should.Throw<FormatException>(() => IPNetworkExtensions.Parse("192.168.0.22/128.0.1.0"));
    }

    #endregion
}
