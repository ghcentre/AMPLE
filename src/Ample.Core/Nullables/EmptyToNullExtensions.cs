using System.Diagnostics.CodeAnalysis;

namespace Ample.Core.Nullables;

/// <summary>
/// Provides extension methods for converting empty or whitespace strings and empty collections to <see langword="null"/> values,
/// and for checking whether a collection is <see langword="null"/> or empty.
/// </summary>
public static class EmptyToNullExtensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="string"/> type
    /// to convert empty or whitespace strings to <see langword="null"/> values.
    /// </summary>
    /// <param name="source">The string to evaluate for emptiness or <see langword="null"/>.</param>
    extension(string? source)
    {
        /// <summary>
        /// Returns <see langword="null"/> if the specified string is <see langword="null"/> or empty; otherwise, returns
        /// the original string.
        /// </summary>
        /// <returns>The original string if it is not <see langword="null"/> or empty;
        /// otherwise, <see langword="null"/>.</returns>
        public string? EmptyToNull()
        {
            return string.IsNullOrEmpty(source) ? null : source;
        }

        /// <summary>
        /// Returns <see langword="null"/> if the specified string is <see langword="null"/>,
        /// empty, or consists only of white-space characters;
        /// otherwise, returns the original string.
        /// </summary>
        /// <returns>The original string if it not <see langword="null"/> and contains non-white-space characters;
        /// otherwise, <see langword="null"/>.</returns>
        public string? WhiteSpaceToNull()
        {
            return string.IsNullOrWhiteSpace(source) ? null : source;
        }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="IEnumerable{T}"/> type to convert empty sequences
    /// to <see langword="null"/> values.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The sequence to check for emptiness or <see langword="null"/>.</param>
    extension<T>(IEnumerable<T>? source)
    {
        /// <summary>
        /// Returns <see langword="null"/> if the specified sequence is <see langword="null"/> or contains no elements;
        /// otherwise, returns the original sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <returns>The original sequence if it contains at least one element; otherwise, <see langword="null"/>.</returns>
        public IEnumerable<T>? EmptyToNull()
        {
            return source.IsNullOrEmpty() ? null : source;
        }
    }

    /// <summary>
    /// Determines whether the specified sequence is <see langword="null"/> or contains no elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The sequence to check for nullity or emptiness.</param>
    /// <returns><see langword="true"/> if the sequence is null or contains no elements;
    /// otherwise, <see langword="false"/>.</returns>
    /// <remarks>The method assumes the collection in <paramref name="source"/> can be <b>safely</b>
    /// enumerated multiple times.</remarks>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? source)
    {
        return source is null || !source.Any();
    }
}
