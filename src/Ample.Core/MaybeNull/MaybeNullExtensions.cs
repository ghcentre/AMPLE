namespace Ample.Core.MaybeNull;

public static class MaybeNullExtensions
{
    public static TResult? Map<TSource, TResult>(this TSource? source, Func<TSource, TResult?> mapper)
        where TResult : notnull
        where TSource : notnull
    {
        if (source is null)
        {
            return default;
        }

        ArgumentNullException.ThrowIfNull(mapper);
        return mapper(source);
    }

    public static TResult? Map<TSource, TResult>(this TSource? source, Func<TSource, TResult?> mapper, Func<TResult?> fallback)
        where TResult : notnull
        where TSource : notnull
    {
        if (source is null)
        {
            ArgumentNullException.ThrowIfNull(fallback);
            return fallback();
        }

        ArgumentNullException.ThrowIfNull(mapper);
        return mapper(source);
    }

    public static TSource? Do<TSource>(this TSource? source, Action<TSource> action)
        where TSource : notnull
    {
        if (source is not null)
        {
            ArgumentNullException.ThrowIfNull(action);
            action(source);
        }
        return source;
    }

    public static TSource? Do<TSource>(this TSource? source, Action<TSource> action, Action fallback)
        where TSource : notnull
    {
        if (source is not null)
        {
            ArgumentNullException.ThrowIfNull(action);
            action(source);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(fallback);
            fallback();
        }

        return source;
    }

    public static TSource? If<TSource>(this TSource? source, Func<TSource, bool> predicate)
        where TSource : notnull
    {
        if (source is null)
        {
            return default;
        }

        ArgumentNullException.ThrowIfNull(predicate);
        return predicate(source) ? source : default;
    }
}