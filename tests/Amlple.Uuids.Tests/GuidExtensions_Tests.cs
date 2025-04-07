using Ample.Uuids;
using Shouldly;

namespace Amlple.Uuids.Tests;

public class GuidExtensions_Tests
{
    [Theory]
    [MemberData(nameof(KnownGuids))]
    public void ToCompactGuid_KnownGuid_ConvertsToKnownString(string guidString, string compactGuidString)
    {
        var sut = new Guid($"{{{guidString}}}");
        var result = sut.ToCompactString();
        result.ShouldBe(compactGuidString);
    }

    public static readonly TheoryData<string, string> KnownGuids = new()
    {
        { "03020100-0504-0706-0809-0a0b0c0d0e0f", "AAECAwQFBgcICQoLDA0ODw" },
        { "00000000-0000-0000-0000-000000000000", "AAAAAAAAAAAAAAAAAAAAAA" },
        { "ffffffff-ffff-ffff-ffff-ffffffffffff", "_____________________w" },
    };
}
