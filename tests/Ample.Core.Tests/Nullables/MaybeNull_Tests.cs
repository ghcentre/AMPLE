using Ample.Core.Nullables;

namespace Ample.Core.Tests.Nullables;

public class MaybeNull_Tests
{
    #region Map(func)

    [Fact]
    public void Map_ReferenceSourceIsNull_ReturnsNull()
    {
        string? source = null;
        var result = source.Map(s => s?.ToUpper());
        result.ShouldBeNull();
    }

    [Fact]
    public void Map_ReferenceSourceIsNotNull_ReturnsMappedValue()
    {
        string source = "test";
        var result = source.Map(s => s.ToUpper());
        result.ShouldBe("TEST");
    }

    [Theory]
    [InlineData(default(int), 1)]
    [InlineData(42, 43)]
    public void Map_ValueSource_AlwaysReturnsMapped(int source, int mapped)
    {
        var result = source.Map(s => s + 1);
        result.ShouldBe(mapped);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(42, 43)]
    public void Map_NullableValueSource_ReturnsNullIfNull(int? source, int? mapped)
    {
        var result = source.Map(s => s + 1);
        result.ShouldBe(mapped);
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData("test", 1)]
    public void Map_ReferenceSource_CountInvocations(string? source, int expectedInvocations)
    {
        int actualInvocations = 0;
        string? result = source.Map(
            s =>
            {
                actualInvocations++;
                return s;
            });
        actualInvocations.ShouldBe(expectedInvocations);
    }

    [Theory]
    [InlineData(default(int), 1)]
    [InlineData(42, 1)]
    public void Map_ValueSource_CountInvocations(int source, int expectedInvocations)
    {
        int actualInvocations = 0;
        var result = source.Map(
            s =>
            {
                actualInvocations++;
                return s;
            });
        actualInvocations.ShouldBe(expectedInvocations);
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData(42, 1)]
    public void Map_NullableValueSource_CountInvocations(int? source, int expectedInvocations)
    {
        int actualInvocations = 0;
        var result = source.Map(
            s =>
            {
                actualInvocations++;
                return s;
            });
        actualInvocations.ShouldBe(expectedInvocations);
    }

    #endregion

    #region Map(func, fallback)

    [Fact]
    public void MapWithFallback_NullSource_ReturnsFallback()
    {
        string? source = null;
        var result = source!.Map(s => s.ToUpper(), () => "null string");
        result.ShouldBe("null string");
    }

    [Fact]
    public void MapWithFallback_NullSource_ThrowsIfNullFallback()
    {
        string? source = null;
        var action = () => source!.Map(s => s.ToUpper(), null!);
        action.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void MapWithFallback_NotNullSource_ThrowsIfNullMapper()
    {
        string source = "test";
        var action = () => source.Map(null!, () => "null string");
        action.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null, 0, 1)]
    [InlineData("test", 1, 0)]
    public void MapWithFallback_CountInvocations(string? source, int expectedMapperInvocations, int expectedFallbackInvocations)
    {
        int actualMapperInvocations = 0;
        int actualFallbackInvocations = 0;

        var result = source.Map(
            s =>
            {
                actualMapperInvocations++;
                return s;
            },
            () =>
            {
                actualFallbackInvocations++;
                return "null string";
            });

        actualMapperInvocations.ShouldBe(expectedMapperInvocations);
        actualFallbackInvocations.ShouldBe(expectedFallbackInvocations);
    }

    [Theory]
    [InlineData(null, "mapper result", "fallback result")]
    [InlineData("test", "mapper result", "fallback result")]
    public void MapWithFallback_MapsNotNullWithMapperAndNullWithFallback(string? source, string? expectedMapped, string? expectedFalledBack)
    {
        var result = source.Map(s => expectedMapped, () => expectedFalledBack);
        result.ShouldBe(
            source is not null
                ? expectedMapped
                : expectedFalledBack);
    }

    #endregion

    #region Do(source, action)

    [Theory]
    [InlineData(null)]
    [InlineData("test string")]
    public void Do_AnySource_ReturnsReferenceEqualsToSource(string? source)
    {
        var result = source.Do(s => { });
        result.ShouldBeSameAs(source);
    }

    [Fact]
    public void Do_NotNullSourceNullAction_Throws()
    {
        string source = "test";
        var action = () => source.Do(null!);
        action.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData("test", 1)]
    public void Do_CountActions(string? source, int expectedInvocations)
    {
        int actualInvocations = 0;
        string? result = source.Do(s => actualInvocations++);
        actualInvocations.ShouldBe(expectedInvocations);
    }

    #endregion

    #region Do(action, fallback)

    [Theory]
    [InlineData(null, 0, 1)]
    [InlineData("test", 1, 0)]
    public void Do_WithFallback_CountActions(string? source, int expectedActionInvocations, int expectedFallbackInvocations)
    {
        int actualActionInvocations = 0;
        int actualFallbackInvocations = 0;

        var result = source.Do(s => actualActionInvocations++, () => actualFallbackInvocations++);

        actualActionInvocations.ShouldBe(expectedActionInvocations);
        actualFallbackInvocations.ShouldBe(expectedFallbackInvocations);
        result.ShouldBeSameAs(source);
    }

    [Fact]
    public void Do_WithFallback_NullAction_Throws()
    {
        string source = "test";
        var action = () => source.Do(null!, () => { });
        action.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Do_WithFallback_NullFallback_Throws()
    {
        string? source = null;
        var action = () => source.Do(s => { }, null!);
        action.ShouldThrow<ArgumentNullException>();
    }

    #endregion

    #region If(predicate)

    [Theory]
    [InlineData(null, false)]
    [InlineData("test", true)]
    [InlineData("", false)]
    public void If_Predicate_ReturnsSourceIfPredicateTrueElseNull(string? source, bool predicateResult)
    {
        var result = source.If(s => !string.IsNullOrEmpty(s));
        if (predicateResult)
        {
            result.ShouldBe(source);
        }
        else
        {
            result.ShouldBeNull();
        }
    }

    [Fact]
    public void If_NullPredicate_Throws()
    {
        string source = "test";
        var action = () => source.If(null!);
        action.ShouldThrow<ArgumentNullException>();
    }

    #endregion
}
