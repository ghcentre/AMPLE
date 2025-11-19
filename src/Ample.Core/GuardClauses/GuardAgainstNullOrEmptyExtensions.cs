using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ample.Core.GuardClauses;

/// <summary>
/// Provides extension methods for guarding against <see langword="null"/>, empty,
/// or whitespace string arguments in method calls.
/// </summary>
public static class GuardAgainstNullOrEmptyExtensions
{
#pragma warning disable IDE0060 // Remove unused parameter

    /// <summary>
    /// Ensures that the specified string argument is not <see langword="null"/> or empty,
    /// throwing an exception if the validation fails.
    /// </summary>
    /// <param name="guard">The guard instance used to invoke the extension method. This parameter is not used.</param>
    /// <param name="argument">The string value to validate. Must not be <see langword="null"/> or empty.</param>
    /// <param name="paramName">The name of the parameter to include in the exception message if the argument
    /// is <see langword="null"/> or empty. This parameter is optional.</param>
    /// <returns>The validated string argument if it is not <see langword="null"/> or empty.</returns>
    /// <exception cref="ArgumentNullException">The string provided in <paramref name="argument"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The string provided in <paramref name="argument"/> is empty.</exception>
    public static string NullOrEmpty(
        this IGuard guard,
        [NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
        return argument;
    }

    /// <summary>
    /// Ensures that the specified string argument is not <see langword="null"/>, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="guard">The guard instance used to invoke the extension method. This parameter is not used.</param>
    /// <param name="argument">The string value to validate. Must not be <see langword="null"/>, empty, or whitespace.</param>
    /// <param name="paramName">The name of the parameter to include in the exception message if the argument
    /// is <see langword="null"/>, empty, or whitespace. This parameter is optional.</param>
    /// <returns>The validated string argument if it is not <see langword="null"/>, empty, or whitespace.</returns>
    /// <exception cref="ArgumentNullException">The string provided in <paramref name="argument"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The string provided in <paramref name="argument"/> is empty or whitespace.</exception>
    public static string NullOrWhiteSpace(
        this IGuard guard,
        [NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument, paramName);
        return argument;
    }

#pragma warning restore IDE0060 // Remove unused parameter
}
