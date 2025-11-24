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
    /// Maps (projects) one object to another if the source object is not <see langword="null"/>. 
    /// </summary>
    /// <typeparam name="TSource">The type of the <paramref name="source"/></typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="source">The object to map. The object can be <see langword="null"/>.</param>
    /// <param name="mapper">The mapper delegate which maps the source to the result.
    /// The delegate has the <see cref="Func{TSource, TResult}"/> type. This parameter cannot be <see langword="null"/>.</param>
    /// <returns>If the object specified in <paramref name="source"/> is <see langword="null"/>,
    /// the method returns the default value for the <typeparamref name="TResult"/> type.
    /// Otherwise, the method returns the result of <paramref name="mapper"/> invocation on the <paramref name="source"/>
    /// object.</returns>
    /// <remarks>For value types, the method always invokes <paramref name="mapper"/> delegate
    /// on the <paramref name="source"/> object.</remarks>
    [return: MaybeNull]
    public static TResult Map<TSource, TResult>([MaybeNull] this TSource source, Func<TSource, TResult> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        return source is null ? default : mapper(source);
    }

    [return: MaybeNull]
    public static TResult Map<TSource, TResult>([MaybeNull] this TSource source, Func<TSource, TResult> mapper, Func<TResult> fallback)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(fallback);

        return source is null ? fallback() : mapper(source);
    }

    [return: NotNullIfNotNull(nameof(source))]
    public static TSource Do<TSource>([MaybeNull] this TSource source, Action<TSource> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        if (source is not null)
        {
            action(source);
        }
        
        return source;
    }

    [return: NotNullIfNotNull(nameof(source))]
    public static TSource Do<TSource>([MaybeNull] this TSource source, Action<TSource> action, Action fallback)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(fallback);
        
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

    [return: MaybeNull]
    public static TSource If<TSource>([MaybeNull] this TSource source, Func<TSource, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return source is null || !predicate(source) ? default : source;
    }
}