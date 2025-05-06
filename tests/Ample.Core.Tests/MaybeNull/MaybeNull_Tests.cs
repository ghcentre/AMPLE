using Ample.Core.MaybeNull;

namespace Ample.Core.Tests.MaybeNull;

public class MaybeNull_Tests
{
    #region Map(source, func)

    [Fact]
    public void Map_NullSource_ReturnsNull()
    {
        string? source = null;
        var result = source.Map(s => s.ToUpper());
        result.ShouldBeNull();
    }

    [Fact]
    public void Map_NullSource_DoesNotThrowIfNullMapper()
    {
        string? source = null;
        var action = () => source.Map((Func<string, string?>)null!);
        action.ShouldNotThrow();
    }

    [Fact]
    public void Map_NotNullSourceNullMapper_Throws()
    {
        string source = "test";
        var action = () => source.Map((Func<string, string?>)null!);
        action.ShouldThrow<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData("test", 1)]
    public void Map_CountInvocations(string? source, int expectedInvocations)
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

    [Fact]
    public void Map_SourceIsNull_ReturnsDefault()
    {
        string? source = null;
        var result = source.Map(s => s.ToUpper());
        result.ShouldBeNull();
    }

    [Fact]
    public void Map_SourceIsNotNull_ReturnsMappedValue()
    {
        string source = "test";
        var result = source.Map(s => s.ToUpper());
        result.ShouldBe("TEST");
    }

    #endregion

    #region Map(source, func, fallback)

    [Fact]
    public void MapWithFallback_NullSource_ReturnsFallback()
    {
        string? source = null;
        var result = source.Map(s => s.ToUpper(), () => "null string");
        result.ShouldBe("null string");
    }

    [Fact]
    public void MapWithFallback_NullSource_DoesNotThrowIfNullMapper()
    {
        string? source = null;
        var action = () => source.Map(null!, () => "null string");
        action.ShouldNotThrow();
    }

    [Fact]
    public void MapWithFallback_NullSource_ThrowsIfNullFallback()
    {
        string? source = null;
        var action = () => source.Map(s => s.ToUpper(), null!);
        action.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void MapWithFallback_NotNullSource_ThrowsIfNullMapper()
    {
        string source = "test";
        var action = () => source.Map(null!, () => "null string");
        action.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void MapWithFallback_NotNullSource_DoesNotThrowIfNullFallback()
    {
        string source = "test";
        var action = () => source.Map(s => s.ToUpper(), null!);
        action.ShouldNotThrow();
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
    public void Do_NullSource_DoesNotThrowIfNullAction()
    {
        string? source = null;
        var action = () => source.Do(null!);
        action.ShouldNotThrow();
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
}
