using Ample.Core.Tests.Uuids.Infrastrusture;
using Ample.Core.Uuids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public void ToCompactGuid_SequentialGuidsV7_CreatesSequentialResults()
    {
        var sut1 = Guid.CreateVersion7();
        var sut2 = Guid.CreateVersion7();

        var results = new[] { sut1.ToCompactString(), sut2.ToCompactString() };
        var sorted = results.OrderBy(x => x).ToArray();

        results.ShouldBe(sorted);
    }

    [Fact]
    public void ToBase32String_Some_Converts()
    {
        var sut = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
        var result = sut.ToBase32String();
        result.Length.ShouldBe(26);
    }

    [Fact]
    public void ToBase32String_Sequential()
    {
        var sut1 = Guid.CreateVersion7();
        var sut2 = Guid.CreateVersion7();
        
        var result1 = sut1.ToBase32String();
        var result2 = sut2.ToBase32String();
        var results = new[] { result1, result2 };
        var sorted = results.OrderBy(x => x).ToArray();

        results.ShouldBe(sorted);
    }
}