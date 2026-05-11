using Ample.Core.GuardClauses;
using System.Diagnostics.CodeAnalysis;

namespace Ample.Core.Nullables;

/// <summary>
/// Provides extension methods for safely operating on potentially null values using functional-style patterns.
/// </summary>
/// <remarks>These methods enable fluent handling of nullable values, allowing mapping, conditional execution, and
/// filtering without explicit null checks. They are particularly useful for scenarios where null values are common and
/// you want to avoid repetitive null-checking logic. All methods are static and intended to be used as extension
/// methods on reference or nullable value types.</remarks>
public static class MaybeNullExtensions
{
    /// <summary>
    /// TODO: Add documentation for this method.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    extension<TSource>([MaybeNull] TSource source)
    {
        /// <summary>
        /// Maps (projects) one object to another if the source object is not <see langword="null"/>. 
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="mapper">The mapper delegate which maps the source to the result.
        /// The delegate has the <see cref="Func{TSource, TResult}"/> type. This parameter cannot be <see langword="null"/>.</param>
        /// <returns>If the object specified in <paramref name="source"/> is <see langword="null"/>,
        /// the method returns the default value for the <typeparamref name="TResult"/> type.
        /// Otherwise, the method returns the result of <paramref name="mapper"/> invocation on the <paramref name="source"/>
        /// object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="mapper"/> is <see langword="null"/>.</exception>
        /// <remarks>For value types, the method always invokes <paramref name="mapper"/> delegate
        /// on the <paramref name="source"/> object.</remarks>
        [return: MaybeNull]
        public TResult Map<TResult>(Func<TSource, TResult> mapper)
        {
            Guard.Against.Null(mapper);
            return source is null ? default : mapper(source);
        }

        /// <summary>
        /// Applies the specified mapping function to the source value if it is not null; otherwise, returns the result of
        /// the fallback function.
        /// </summary>
        /// <remarks>This method is useful for safely mapping nullable values without explicit null checks. Both
        /// <paramref name="mapper"/> and <paramref name="fallback"/> must be provided; otherwise, an <see
        /// cref="ArgumentNullException"/> is thrown.</remarks>
        /// <typeparam name="TResult">The type of the result produced by the mapping or fallback function.</typeparam>
        /// <param name="mapper">A function that maps the source value to a result. Cannot be <see langword="null"/>.</param>
        /// <param name="fallback">A function that provides a fallback result when the source value is null. Cannot be null.</param>
        /// <returns>The result of applying <paramref name="mapper"/> to <paramref name="source"/> if it is not null; otherwise, the
        /// result of <paramref name="fallback"/>.</returns>
        [return: MaybeNull]
        public TResult Map<TResult>(Func<TSource, TResult> mapper, Func<TResult> fallback)
        {
            Guard.Against.Null(mapper);
            Guard.Against.Null(fallback);

            return source is null ? fallback() : mapper(source);
        }

        /// <summary>
        /// Executes the specified action if the source object is not <see langword="null"/>.
        /// The source object is returned unchanged.
        /// </summary>
        /// <param name="action">The function to execute.</param>
        /// <returns>The <paramref name="source"/>, unchanged.</returns>
        [return: NotNullIfNotNull(nameof(source))]
        public TSource Do(Action<TSource> action)
        {
            Guard.Against.Null(action);

            if (source is not null)
            {
                action(source);
            }

            return source;
        }

        /// <summary>
        /// Invokes the specified action if the source value is not null; otherwise, invokes the fallback action.
        /// Returns the original source value.
        /// </summary>
        /// <param name="action">The action to perform if the source value is not null. The source value is passed as an argument to this
        /// action. Cannot be null.</param>
        /// <param name="fallback">The action to perform if the source value is null. Cannot be null.</param>
        /// <returns>The original source value. If the source is null, returns null.</returns>
        [return: NotNullIfNotNull(nameof(source))]
        public TSource Do(Action<TSource> action, Action fallback)
        {
            Guard.Against.Null(action);
            Guard.Against.Null(fallback);

            if (source is not null)
            {
                action(source);
            }
            else
            {
                fallback();
            }

            return source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        [return: MaybeNull]
        public TSource If(Func<TSource, bool> predicate)
        {
            Guard.Against.Null(predicate);
            return source is null || !predicate(source) ? default : source;
        }
    }
}