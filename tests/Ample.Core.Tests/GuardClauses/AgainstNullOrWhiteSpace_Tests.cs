using Ample.Core.GuardClauses;

namespace Ample.Core.Tests.GuardClauses;

public class AgainstNullOrWhiteSpace_Tests
{
    [Fact]
    public void NotNullOrWhiteSpace_DoesNotThrow()
    {
        string arg = "test";
        Action sut = () => Guard.Against.NullOrWhiteSpace(arg);
        sut.ShouldNotThrow();
    }

    [Fact]
    public void NotNullOrEmpty_Null_ThrowsArgumentNullException()
    {
        string? arg = null;
        Action sut = () => Guard.Against.NullOrWhiteSpace(arg);
        sut.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void NotNullOrEmpty_Empty_ThrowsArgumentException()
    {
        string arg = string.Empty;
        Action sut = () => Guard.Against.NullOrWhiteSpace(arg);
        sut.ShouldThrow<ArgumentException>();
    }
}
