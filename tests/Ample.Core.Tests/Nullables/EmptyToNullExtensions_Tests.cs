using Ample.Core.Nullables;

namespace Ample.Core.Tests.Nullables;

public class EmptyToNullExtensions_Tests
{
    #region Infrastructure
    
    private static IEnumerable<int> EmptyYield()
    {
        yield break;
    }

    private static IEnumerable<int> NonEmptyYield()
    {
        yield return 42;
    }

    #endregion

    #region IsNullOrEmpty (enumerable)

    [Fact]
    public void IsNullOrEmpty_Null_RetursTrue()
    {
        IEnumerable<int>? source = null;
        source.IsNullOrEmpty().ShouldBeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_Empty_RetursTrue()
    {
        IEnumerable<int> emptyArray = Array.Empty<int>();
        IEnumerable<int> emptyList = [];
        IEnumerable<int> emptyYield = EmptyYield();

        emptyArray.IsNullOrEmpty().ShouldBeTrue();
        emptyList.IsNullOrEmpty().ShouldBeTrue();
        emptyYield.IsNullOrEmpty().ShouldBeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_NonEmpty_RetursFalse()
    {
        IEnumerable<int> fromArray = new[] { 1 };
        IEnumerable<int> fromList = new List<int>() { 1 };
        IEnumerable<int> fromYield = NonEmptyYield();

        fromArray.IsNullOrEmpty().ShouldBeFalse();
        fromList.IsNullOrEmpty().ShouldBeFalse();
        fromYield.IsNullOrEmpty().ShouldBeFalse();
    }

    #endregion

    #region EmptyToNull (enumerable)

    [Fact]
    public void EmptyToNull_Null_RetursNull()
    {
        IEnumerable<string>? source = null;
        source.EmptyToNull().ShouldBeNull();
    }

    [Fact]
    public void EmptyToNull_Empty_RetursNull()
    {
        IEnumerable<string> empty = [];
        empty.EmptyToNull().ShouldBeNull();
    }

    [Fact]
    public void EmptyToNull_NonEmpty_RetursOriginal()
    {
        var list = new List<string>() { "a" };
        var result = list.EmptyToNull();
        result.ShouldBeSameAs(list);

        var array = new[] { "b" };
        var result2 = array.EmptyToNull();
        result2.ShouldBeSameAs(array);
    }

    #endregion

    #region EmptyToNull

    [Fact]
    public void EmptyToNull_EmptyString_ReturnsNull()
    {
        string? input = string.Empty;
        var result = input.EmptyToNull();
        result.ShouldBeNull();
    }

    [Fact]
    public void EmptyToNull_NullString_ReturnsNull()
    {
        string? input = null;
        var result = input.EmptyToNull();
        result.ShouldBeNull();
    }

    [Fact]
    public void EmptyToNull_NonEmptyString_ReturnsOriginalString()
    {
        string? input = "Hello, World!";
        var result = input.EmptyToNull();
        result.ShouldBe(input);
    }

    #endregion

    #region WhiteSpaceToNull

    [Fact]
    public void WhiteSpaceToNull_WhiteSpaceString_ReturnsNull()
    {
        string? input = "   ";
        var result = input.WhiteSpaceToNull();
        result.ShouldBeNull();
    }

    [Fact]
    public void WhiteSpaceToNull_NullString_ReturnsNull()
    {
        string? input = null;
        var result = input.WhiteSpaceToNull();
        result.ShouldBeNull();
    }

    [Fact]
    public void WhiteSpaceToNull_NonWhiteSpaceString_ReturnsOriginalString()
    {
        string? input = "Hello, World!";
        var result = input.WhiteSpaceToNull();
        result.ShouldBe(input);
    }

    #endregion
}