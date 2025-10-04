using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ample.Core.GuardClauses;

public static class GuardAgainstOutOfRangeExtensions
{
#pragma warning disable IDE0060 // Remove unused parameter
    public static T Equal<T>(
        this IGuard guard,
        T value,
        T other,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IEquatable<T>?
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, other, paramName);
        return value;
    }

    public static T GreaterThan<T>(
        this IGuard guard,
        T value,
        T other,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);
        return value;
    }

    public static T GreaterThanOrEqual<T>(
        this IGuard guard,
        T value,
        T other,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);
        return value;
    }

    public static T LessThan<T>(
        this IGuard guard,
        T value,
        T other,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);
        return value;
    }

    public static T LessThanOrEqual<T>(
        this IGuard guard,
        T value,
        T other,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);
        return value;
    }

    public static T NotEqual<T>(
        this IGuard guard,
        T value,
        T other,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IEquatable<T>?
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);
        return value;
    }


    public static T Negative<T>(
        this IGuard guard,
        T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : INumberBase<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
        return value;
    }

    public static T NegativeOrZero<T>(
        this IGuard guard,
        T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : INumberBase<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
        return value;
    }

    public static T Zero<T>(
        this IGuard guard,
        T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : INumberBase<T>
    {
        ArgumentOutOfRangeException.ThrowIfZero(value, paramName);
        return value;
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
