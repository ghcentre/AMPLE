using Ample.Core.GuardClauses;

namespace Ample.Core.Tests.GuardClauses;

public class AgainstNull_Tests
{
    [Fact]
    public void Reference_Any_DoesNotThrow()
    {
        int arg = 42;
        Action sut = () => Guard.Against.Null(arg);
        sut.ShouldNotThrow();
    }

    [Fact]
    public void NullableReference_Null_Throws()
    {
        int? arg = null;
        Action sut = () => Guard.Against.Null(arg);
        sut.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void NullableReference_NotNull_DoesNotThrow()
    {
        int? arg = 42;
        Action sut = () => Guard.Against.Null(arg);
        sut.ShouldNotThrow();
    }
}
