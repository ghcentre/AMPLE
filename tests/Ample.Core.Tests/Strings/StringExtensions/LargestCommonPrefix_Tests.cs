using Ample.Core.Strings;

namespace Ample.Core.Tests.Strings.StringExtensions;

public class LargestCommonPrefix_Tests
{
    public static TheoryData<string, string[]> LargestCommonPrefixData = new()
    {
        { "this is a", [ "this is a test",
                         "this is a test 2",
                         "this is another test"] },
        { "",          [ "one",
                         "two"] },
        { "",          [ "mama loves me",
                         "papa loves me" ] },
        { "my ",       [ "my mama loves me",
                         "my papa loves me" ] },
        { "",          [ "my mama loves me",
                          "",
                         "my papa loves me" ] },
        { "aga",       [ "aga",
                         "aga" ] }
    };

    [Fact]
    public void NullInput_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => ((string[])null!).LongestCommonPrefixIndex());
    }

    [Fact]
    public void NullStringInInput_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new[] { "valid", null!, "also valid" }.LongestCommonPrefixIndex());
    }

    [Fact]
    public void EmptyArray_ReturnsZero()
    {
        var result = Enumerable.Empty<string>().LongestCommonPrefixIndex();
        result.ShouldBe(0);
    }

    [Theory]
    [MemberData(nameof(LargestCommonPrefixData))]
    public void ValidData_ReturnsExpected(string expected, string[] args)
    {
        var result = args.LongestCommonPrefixIndex();
        var resultString = args[0][..result];

        resultString.ShouldBe(expected);
    }
}
