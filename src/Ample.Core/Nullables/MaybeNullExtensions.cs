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