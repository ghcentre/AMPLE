using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ample.Core.GuardClauses;

public static class GuardAgainstNullOrEmptyExtensions
{
#pragma warning disable IDE0060 // Remove unused parameter
    public static string NullOrEmpty(
        this IGuard guard,
        [NotNull] string? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
        return argument;
    }

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
