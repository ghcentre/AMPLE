using Ample.Core.Tests.Uuids.Infrastrusture;
using Ample.Core.Uuids;

namespace Ample.Core.Tests.Uuids;

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

    public static TheoryData<string, string> KnownGuids => TestTheoryData.KnownGuids;

    #endregion

    #region TryParseCompact

    [Theory]
    [MemberData(nameof(NullOrEmptyStrings))]
    public void TryParseCompact_NullOrEmpty_ReturnsFalse(string? inputString)
    {
        var sut = inputString;
        bool result = Guid.TryParseCompact(sut, out _);
        result.ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(InvalidLengthStrings))]
    public void TryParseCompact_InvalidLength_ReturnsFalse(string? inputString)
    {
        var sut = inputString;
        bool result = Guid.TryParseCompact(sut, out _);
        result.ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(InvalidStrings))]
    public void TryParseCompact_InvalidData_ReturnsFalse(string? inputString)
    {
        var sut = inputString;
        bool result = Guid.TryParseCompact(sut, out _);
        result.ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(KnownGuids))]
    public void TryParseCompact_KnownStrings_ReturnsTrueAndOutputsKnownGuid(string guidString, string compactGuidString)
    {
        var sut = compactGuidString;
        var success = Guid.TryParseCompact(sut, out var result);
        success.ShouldBeTrue();
        result.ToString().ShouldBe(guidString);
    }

    #endregion

    #region ParseCompact

    [Theory]
    [MemberData(nameof(NullOrEmptyStrings))]
    public void ParseCompact_NullOrEmpty_Throws(string? inputString)
    {
        var sut = inputString;
        Should.Throw<FormatException>(() => Guid.ParseCompact(sut));
    }

    [Theory]
    [MemberData(nameof(InvalidLengthStrings))]
    public void ParseCompact_InvalidLength_ReturnsFalse(string? inputString)
    {
        var sut = inputString;
        Should.Throw<FormatException>(() => Guid.ParseCompact(sut));
    }

    [Theory]
    [MemberData(nameof(InvalidStrings))]
    public void ParseCompact_InvalidData_ReturnsFalse(string? inputString)
    {
        var sut = inputString;
        Should.Throw<FormatException>(() => Guid.ParseCompact(sut));
    }

    [Theory]
    [MemberData(nameof(KnownGuids))]
    public void ParseCompact_KnownStrings_ReturnsTrueAndOutputsKnownGuid(string guidString, string compactGuidString)
    {
        var sut = compactGuidString;
        var result = Guid.ParseCompact(sut);
        result.ToString().ShouldBe(guidString);
    }

    #endregion
}