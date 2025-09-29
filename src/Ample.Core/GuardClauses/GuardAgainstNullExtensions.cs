using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ample.Core.GuardClauses;

public static class GuardAgainstNullExtensions
{
    public static T? Null<T>(
        this IGuard guard,
        [NotNull] T? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument;
    }

    public static T Null<T>(
        this IGuard guard,
        [NotNull] T? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : struct
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument.Value;
    }
}
