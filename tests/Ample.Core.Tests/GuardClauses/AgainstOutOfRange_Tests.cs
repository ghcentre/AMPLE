using Ample.Core.GuardClauses;

namespace Ample.Core.Tests.GuardClauses;

public class AgainstOutOfRange_Tests
{
    #region Equal

    [Fact]
    public void Equal_NotEqual_DoesNotThrow()
    {
        int arg = 42;
        Action sut = () => Guard.Against.Equal(arg, 43);
        sut.ShouldNotThrow();
    }

    [Fact]
    public void Equal_Equal_ThrowsOutOfRangeException()
    {
        int arg = 42;
        Action sut = () => Guard.Against.Equal(arg, 42);
        sut.ShouldThrow<ArgumentOutOfRangeException>();
    }

    #endregion

    #region GreaterThan

    [Fact]
    public void GreaterThan_LessThan_DoesNotThrow()
    {
        double arg = 42.42;
        Action sut = () => Guard.Against.GreaterThan(arg, 43);
        sut.ShouldNotThrow();
    }

    [Fact]
    public void GreaterThan_Equals_DoesNotThrow()
    {
        double arg = 42.42;
        Action sut = () => Guard.Against.GreaterThan(arg, 42.42);
        sut.ShouldNotThrow();
    }

    [Fact]
    public void GreaterThan_GreatherThan_ThrowsOutOfRangeException()
    {
        double arg = 42.42;
        Action sut = () => Guard.Against.GreaterThan(arg, 42.41999999999999);
        sut.ShouldThrow<ArgumentOutOfRangeException>();
    }

    #endregion

    #region GreaterThanOrEqual

    [Fact]
    public void GreaterThanOrEqual_LessThan_DoesNotThrow()
    {
        double arg = 42.42;
        Action sut = () => Guard.Against.GreaterThanOrEqual(arg, 43);
        sut.ShouldNotThrow();
    }

    [Fact]
    public void GreaterThanOrEqual_Equals_ThrowsOutOfRangeException()
    {
        double arg = 42.42;
        Action sut = () => Guard.Against.GreaterThanOrEqual(arg, 42.42);
        sut.ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GreaterThanOrEqual_GreatherThan_ThrowsOutOfRangeException()
    {
        double arg = 42.42;
        Action sut = () => Guard.Against.GreaterThanOrEqual(arg, 42.41999999999999);
        sut.ShouldThrow<ArgumentOutOfRangeException>();
    }

    #endregion
}
