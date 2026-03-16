using Ample.Core.Disposables;

namespace Ample.Core.Tests.Disposables;

public class DisposableBafOfT_Tests
{
    #region Infrastructure

    private readonly List<string> _messages = new();

    private class BagObject
    {
    }

    #endregion

    #region Ctor

    [Fact]
    public void Ctor_Value_SameReference()
    {
        var bagObject = new BagObject();
        var sut = new DisposableBag<BagObject>(bagObject);

        var actual = sut.Value;

        actual.ShouldBe(bagObject);
    }

    [Fact]
    public void Ctor_Null_AllowsNulls()
    {
        var sut = new DisposableBag<string>(null);
        var actual = sut.Value;
        actual.ShouldBeNull();
    }

    #endregion

    #region Add

    [Fact]
    public void Add_Action_Executes()
    {
        void action() => _messages.Add("string");
        var sut = new DisposableBag<string>(null);

        sut.Add(action);
        sut.Dispose();

        _messages[0].ShouldBe("string");
    }

    [Fact]
    public void Add_AfterDispose_Throws()
    {
        void action() => _messages.Add("string");
        var sut = new DisposableBag<string>(null);

        sut.Dispose();

        Should.Throw<ObjectDisposedException>(() => sut.Add(action));
    }

    #endregion

    #region Dispose

    [Fact]
    public void Dispose_ExecutedOnce()
    {
        void action() => _messages.Add("string");
        var sut = new DisposableBag<string>(null);
        sut.Add(action);

        sut.Dispose();
        sut.Dispose();

        _messages.Count.ShouldBe(1);
    }

    [Fact]
    public void Dispose_AddedInOrder_ExecutedInReverseOrder()
    {
        void action1() => _messages.Add("1");
        void action2() => _messages.Add("2");
        void action3() => _messages.Add("3");
        var sut = new DisposableBag<string>(null);
        sut.Add(action1);
        sut.Add(action2);
        sut.Add(action3);

        sut.Dispose();

        _messages[0].ShouldBe("3");
        _messages[1].ShouldBe("2");
        _messages[2].ShouldBe("1");
    }

    #endregion
}
