using Ample.Core.GuardClauses;

namespace Ample.Core.Tests.GuardClauses;

public class AgainstNullOrEmpty_Tests
{
    [Fact]
    public void NotNullOrEmpty_DoesNotThrow()
    {
        string arg = "test";
        Action sut = () => Guard.Against.NullOrEmpty(arg);
        sut.ShouldNotThrow();
    }

    [Fact]
    public void Null_ThrowsArgumentNullException()
    {
        string? arg = null;
        Action sut = () => Guard.Against.NullOrEmpty(arg);
        sut.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Empty_ThrowsArgumentException()
    {
        string arg = string.Empty;
        Action sut = () => Guard.Against.NullOrEmpty(arg);
        sut.ShouldThrow<ArgumentException>();
    }
}
