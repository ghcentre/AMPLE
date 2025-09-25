using Ample.Core.Tests.Uuids.Infrastrusture;
using Ample.Core.Uuids;

namespace Ample.Core.Tests.Uuids;

public class GuidExtensions_Tests
{
    #region TheoryData

    public static TheoryData<string, string> KnownGuids => TestTheoryData.KnownGuids;

    #endregion

    [Theory]
    [MemberData(nameof(KnownGuids))]
    public void ToCompactGuid_KnownGuid_ConvertsToKnownString(string guidString, string compactGuidString)
    {
        var sut = new Guid($"{{{guidString}}}");
        var result = sut.ToCompactString();
        result.ShouldBe(compactGuidString);
    }

    [Fact]
    public void ToCompactGuid_Random_ConvertsToFixedLength()
    {
        for (int i = 0; i < 1000; i++)
        {
            var sut = Guid.NewGuid();
            var result = sut.ToCompactString();
            result.Length.ShouldBe(22);
        }
    }

    [Fact]
    public void ToCompactGuid_Random_ContainsAllowedCharactersOnly()
    {
        const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        for (int i = 0; i < 1000; i++)
        {
            var sut = Guid.NewGuid();
            var result = sut.ToCompactString();
            result.All(x => allowedChars.Any(ac => x == ac)).ShouldBeTrue();
        }
    }

    [Fact]
    public void ToCompactGuidAndBack_Random_BackConvertedEqualsOriginal()
    {
        for (int i = 0; i < 1000; i++)
        {
            var sut = Guid.NewGuid();
            var compactString = sut.ToCompactString();
            var result = CompactGuid.Parse(compactString);
            result.ShouldBe(sut);
        }
    }

    [Fact]
    public void ToBase32String_SomeGuid_CorrectLength()
    {
        var sut = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
        var result = sut.ToBase32String();
        result.Length.ShouldBe(26);
    }

    [Fact]
    public void ToBase32String_Random_ContainsAllowedCharactersOnly()
    {
        const string allowedChars = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

        for (int i = 0; i < 1000; i++)
        {
            var sut = Guid.NewGuid();
            var result = sut.ToBase32String();
            result.All(x => allowedChars.Any(ac => x == ac)).ShouldBeTrue();
        }
    }
}