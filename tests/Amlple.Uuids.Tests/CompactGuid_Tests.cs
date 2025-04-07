using Ample.Uuids;
using Shouldly;

namespace Amlple.Uuids.Tests;

public class CompactGuid_Tests
{
    #region TheoryData

    public static readonly TheoryData<string?> NullOrEmptyStrings = new()
    {
        { "" },
        { (string?)null }
    };

    public static readonly TheoryData<string?> InvalidLengthStrings = new()
    {
        { "AAECAwQFBgcICQoLDA0ODwA" },
        { "AAAAAAAAAAAAAAAAAAAAA" },
        { "______________________w" }
    };

    public static readonly TheoryData<string?> InvalidStrings = new()
    {
        { "AAECAw*FBgcICQoLDA0ODw" },
        { "AAAAAAAAAAAAAAAAAA*AAA" },
        { "_______*_____________w" }
    };

    public static readonly TheoryData<string, string> KnownGuids = new()
    {
        { "03020100-0504-0706-0809-0a0b0c0d0e0f", "AAECAwQFBgcICQoLDA0ODw" },
        { "00000000-0000-0000-0000-000000000000", "AAAAAAAAAAAAAAAAAAAAAA" },
        { "ffffffff-ffff-ffff-ffff-ffffffffffff", "_____________________w" },
    };

    #endregion

    [Theory]
    [MemberData(nameof(NullOrEmptyStrings))]
    public void TryParse_NullOrEmpty_ReturnsFalse(string? inputString)
    {
        var sut = inputString;
        bool result = CompactGuid.TryParse(sut, out _);
        result.ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(InvalidLengthStrings))]
    public void TryParse_InvalidLength_ReturnsFalse(string? inputString)
    {
        var sut = inputString;
        bool result = CompactGuid.TryParse(sut, out _);
        result.ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(InvalidStrings))]
    public void TryParse_InvalidData_ReturnsFalse(string? inputString)
    {
        var sut = inputString;
        bool result = CompactGuid.TryParse(sut, out _);
        result.ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(KnownGuids))]
    public void TryParse_KnownStrings_ReturnsTrueAndOutputsKnownGuid(string guidString, string compactGuidString)
    {
        var sut = compactGuidString;
        var success = CompactGuid.TryParse(sut, out var result);
        success.ShouldBeTrue();
        result.ToString().ShouldBe(guidString);
    }
}
