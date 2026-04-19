using Ample.Core.Disposables;
using Ample.Core.GuardClauses;

namespace Ample.Core.Tests.Disposables;

public class DisposableBag_Tests
{
    #region Infrastructure

    private class DisposableActionWrapper(Action actionOnDispose) : IDisposable
    {
        private readonly Action _actionOnDispose = Guard.Against.Null(actionOnDispose);

        public void Dispose()
        {
            _actionOnDispose();
        }
    }

    #endregion

    #region Create

    [Fact]
    public void Create_WithoutParameter_ValueIsNull()
    {
        var actual = DisposableBag.Create();

        actual.ShouldNotBeNull();
        actual.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithParameter_CreatesNonNull()
    {
        var sut = DisposableBag.Create(42);
        sut.ShouldNotBeNull();
    }

    #endregion

    #region With

    [Fact]
    public void With_AnyArg_ReturnsSameBag()
    {
        var sut = DisposableBag.Create(42);
        var actual = sut.With(() => { });
        actual.ShouldBe(sut);
    }

    [Fact]
    public void With_Disposable_ReturnsSameBag()
    {
        var sut = DisposableBag.Create(42);
        var disposable = new DisposableActionWrapper(() => { });
        var actual = sut.With(disposable);
        actual.ShouldBe(sut);
    }

    #endregion
}
