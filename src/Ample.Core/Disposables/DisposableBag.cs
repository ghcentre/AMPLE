using Ample.Core.GuardClauses;

namespace Ample.Core.Disposables;

/// <summary>
/// Contains extension methods for the <see cref="DisposableBag{T}"/>.
/// </summary>
public static class DisposableBag
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableBag{T}"/> with no assigned value.
    /// </summary>
    /// <returns><see cref="DisposableBag{T}"/> with value assigned to <see langword="null"/>.</returns>
    public static DisposableBag<object> Create()
    {
        return new DisposableBag<object>(null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableBag{T}"/> with the value specified in the parameter.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to store in the bag.</param>
    /// <returns><see cref="DisposableBag{T}"/>.</returns>
    public static DisposableBag<T> Create<T>(T value)
    {
        return new DisposableBag<T>(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableBag{T}"/> with the value specified in the parameter,
    /// and adds the value to the collection of disposables which are disposed when the bag is disposed.
    /// </summary>
    /// <typeparam name="T">The type of the value. The type must implement the <see cref="IDisposable"/> interface.</typeparam>
    /// <param name="value">The value to store in the bag.</param>
    /// <returns><see cref="DisposableBag{T}"/>.</returns>
    public static DisposableBag<T> CreateForDisposable<T>(T value) where T : class, IDisposable
    {
        Guard.Against.Null(value);
        var bag = new DisposableBag<T>(value);
        return bag.With(value);
    }

    /// <summary>
    /// Adds the disposable object to the collection of objects which are disposed when the bag is disposed.
    /// </summary>
    /// <typeparam name="T">The type of the value stored in the bag.</typeparam>
    /// <param name="bag">The bag.</param>
    /// <param name="disposable">The disposable object.</param>
    /// <returns>The bag.</returns>
    /// <exception cref="ArgumentNullException">The bag is <see langword="null"/>,
    /// or the disposable object is <see langword="null"/>.</exception>
    public static DisposableBag<T> With<T>(this DisposableBag<T> bag, IDisposable disposable)
    {
        Guard.Against.Null(bag);
        Guard.Against.Null(disposable);

        bag.Add(disposable.Dispose);

        return bag;
    }

    /// <summary>
    /// Adds the action to the collection of actions which are executed when the bag is disposed.
    /// </summary>
    /// <typeparam name="T">The type of the value stored in the bag.</typeparam>
    /// <param name="bag">The bag.</param>
    /// <param name="action">The action</param>
    /// <returns>The bag.</returns>
    /// <exception cref="ArgumentNullException">The bag is <see langword="null"/>,
    /// or the action is <see langword="null"/>.</exception>
    public static DisposableBag<T> With<T>(this DisposableBag<T> bag, Action action)
    {
        Guard.Against.Null(bag);
        Guard.Against.Null(action);

        bag.Add(action);

        return bag;
    }

    /// <summary>
    /// Contains the extension method for the <see cref="DisposableBag{T}"/> class.
    /// </summary>
    /// <typeparam name="T">The type of the value stored in the bag.</typeparam>
    /// <param name="bag">The <see cref="DisposableBag{T}"/> bag.</param>
    extension<T>(DisposableBag<T> bag)
    {
        /// <summary>
        /// Adds the collection of disposables to the bag. Disposables are disposed when the bag is disposed.
        /// </summary>
        /// <param name="disposables">Sequence of disposables.</param>
        /// <returns>The bag.</returns>
        /// <exception cref="ArgumentNullException">The bag is <see langword="null"/>,
        /// or the sequence of disposables is <see langword="null"/>.</exception>
        public DisposableBag<T> With(params IEnumerable<IDisposable> disposables)
        {
            Guard.Against.Null(bag);
            Guard.Against.Null(disposables);

            foreach (var disposable in disposables)
            {
                bag.With(disposable); // will check for null
            }

            return bag;
        }

        /// <summary>
        /// Adds the collection of actions to the bag. Actions are executed when the bag is disposed.
        /// </summary>
        /// <param name="actions">The sequence of actions.</param>
        /// <returns>The bag.</returns>
        /// <exception cref="ArgumentNullException">The bag is <see langword="null"/>,
        /// or the sequence of actions is <see langword="null"/>.</exception>
        public DisposableBag<T> With(params IEnumerable<Action> actions)
        {
            Guard.Against.Null(bag);
            Guard.Against.Null(actions);

            foreach (var action in actions)
            {
                bag.With(action); // will check for null
            }

            return bag;
        }
    }
}