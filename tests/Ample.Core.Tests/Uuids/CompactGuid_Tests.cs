using Ample.Core.Tests.Uuids.Infrastrusture;
using Ample.Core.Uuids;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    #region TryParse

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

    #endregion

    #region Parse

    [Theory]
    [MemberData(nameof(NullOrEmptyStrings))]
    public void Parse_NullOrEmpty_Throws(string? inputString)
    {
        var sut = inputString;
        Should.Throw<FormatException>(() => CompactGuid.Parse(sut));
    }

    [Theory]
    [MemberData(nameof(InvalidLengthStrings))]
    public void Parse_InvalidLength_ReturnsFalse(string? inputString)
    {
        var sut = inputString;
        Should.Throw<FormatException>(() => CompactGuid.Parse(sut));
    }

    [Theory]
    [MemberData(nameof(InvalidStrings))]
    public void Parse_InvalidData_ReturnsFalse(string? inputString)
    {
        var sut = inputString;
        Should.Throw<FormatException>(() => CompactGuid.Parse(sut));
    }

    [Theory]
    [MemberData(nameof(KnownGuids))]
    public void Parse_KnownStrings_ReturnsTrueAndOutputsKnownGuid(string guidString, string compactGuidString)
    {
        var sut = compactGuidString;
        var result = CompactGuid.Parse(sut);
        result.ToString().ShouldBe(guidString);
    }

    #endregion
}