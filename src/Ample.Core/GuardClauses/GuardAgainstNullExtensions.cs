using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ample.Core.GuardClauses;

/// <summary>
/// Provides extension methods for guarding against <see langword="null"/> arguments in method calls.
/// </summary>
public static class GuardAgainstNullExtensions
{
    extension(IGuard guard)
    {
#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// Ensures that the specified argument is not <see langword="null"/> and throws an <see cref="ArgumentNullException"/> if it is.
        /// </summary>
        /// <typeparam name="T">The type of the argument to validate.</typeparam>
        /// <param name="argument">The argument to validate for <see langword="null"/>. Must not be <see langword="null"/>.</param>
        /// <param name="paramName">The name of the parameter to include in the exception message if the argument is <see langword="null"/>.
        /// This parameter is optional.</param>
        /// <returns>The validated argument if it is not <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="argument"/> is <see langword="null"/>.</exception>
        public T Null<T>(
            [NotNull] T? argument,
            [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);
            return argument;
        }

        /// <summary>
        /// Ensures that the specified nullable value type argument is not null and throws an <see cref="ArgumentNullException"/> if it is.
        /// Returns the non-null value of the argument.
        /// </summary>
        /// <typeparam name="T">The type of the argument to check for null. Only value types are allowed.</typeparam>
        /// <param name="argument">The nullable value type argument to validate. Must have a value.</param>
        /// <param name="paramName">The name of the parameter to include in the exception message if the argument is <see langword="null"/>.
        /// This parameter is optional.</param>
        /// <returns>The non-null value of the specified argument.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="argument"/> is <see langword="null"/>.</exception>
        public static T Null<T>(
            [NotNull] T? argument,
            [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            where T : struct
        {
            if (!argument.HasValue)
            {
                throw new ArgumentNullException(paramName);
            }
            return argument.Value;
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
