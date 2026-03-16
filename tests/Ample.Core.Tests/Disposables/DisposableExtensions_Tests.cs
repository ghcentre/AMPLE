using Ample.Core.Disposables;

namespace Ample.Core.Tests.Disposables;

public class DisposableExtensions_Tests
{
    #region Infrastructure
    
    private class TestDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public int Value { get; set; }
        public void Dispose() => IsDisposed = true;
    }

    #endregion

    [Fact]
    public void Use_ObjectIsNull_ReturnsDefault()
    {
        TestDisposable? sut = null;
        var result = sut.Use(d => d.Value);
        result.ShouldBe(default);
    }

    [Fact]
    public void Use_FuncIsNull_ThrowsArgumentNullException()
    {
        var sut = new TestDisposable();
        Func<TestDisposable, int>? func = null;
        Should.Throw<ArgumentNullException>(() => sut.Use(func!));
    }

    [Fact]
    public void Use_ObjectIsNotNull_InvokesDelegateAndDisposesObject()
    {
        var sut = new TestDisposable { Value = 42 };
        bool disposedBefore = sut.IsDisposed;
        int result = sut.Use(d => d.Value + 1);
        
        disposedBefore.ShouldBeFalse();
        result.ShouldBe(43);
        sut.IsDisposed.ShouldBeTrue();
    }
}
