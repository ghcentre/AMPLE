using Ample.Core.GuardClauses;

namespace Ample.Core.Disposables;

/// <summary>
/// Provides a container for an arbitrary value and a collection of <see cref="Action"/>s.
/// When the container is disposed, all actions are executed.
/// </summary>
/// <typeparam name="T">The type of the object.</typeparam>
/// <seealso cref="DisposableBag"/>
/// <remarks>
/// <para>Use the <see cref="Add(Action)"/> method to add an action to the collection of actions
/// which are executed when the container is disposed.</para>
/// <para>Last action added is executed first.</para>
/// <para>To dispose the object on bag's disposal add the object's <c>Dispose</c> method:
/// <code>bag.Add(disposable.Dispose);</code>,
/// or use the <see cref="DisposableBag.With{T}(DisposableBag{T}, IDisposable)"/>
/// extension method.
/// </para>
/// </remarks>
/// <example>
/// <code><![CDATA[
/// private SomeClass CreateWithDependencies()
/// {
///     var someDependency = new SomeDependency();
///     var anotherDependency = new AnotherDependency(someDependency);
///     var someClass = new SomeClass(anotherDependency);
///     var bag = DisposableBag.Create(someClass).With(someDependency, anotherDependency);
///     return bag;
/// }
/// 
/// using (var someClassBag = _someClassBagFactory())
/// {
///     var someClass = someClassBag.Value;
///     // do something with someClass
/// 
///     // disposables added to the bag are disposed in the reverse order:
///     // anotherDependency.Dispose()
///     // someDependency.Dispose()
/// }
/// ]]></code>
/// </example>
/// <remarks>
/// Initializes a new instance of the <see cref="DisposableBag{T}"/> class.
/// </remarks>
/// <param name="value">The value to store in the bag.</param>
public class DisposableBag<T>(T? value)
{
    private readonly Stack<Action> _actions = new();


    /// <summary>
    /// Adds a <see cref="Action"/> to the collection of actions which are executed when the container is disposed.
    /// </summary>
    /// <param name="action">The action to add.</param>
    public virtual void Add(Action action)
    {
        Guard.Against.Null(action);

        ObjectDisposedException.ThrowIf(_disposed, this);
        _actions.Push(action);
    }


    /// <summary>
    /// Gets the value stored in the bag.
    /// </summary>
    /// <value>The value stored in the bag.</value>
    public T? Value { get; } = value;


    #region IDisposable

    private bool _disposed = false;

    /// <summary>
    /// Performs the object cleanup.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> if disposing managed resources,
    /// or <see langword="false"/> otherwise.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {
            foreach (var action in _actions)
            {
                action();
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
