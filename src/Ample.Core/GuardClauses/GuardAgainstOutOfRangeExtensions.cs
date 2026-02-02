using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ample.Core.GuardClauses;

public static class GuardAgainstOutOfRangeExtensions
{
    extension(IGuard guard)
    {

#pragma warning disable CA1822 // Mark members as static

        public T Equal<T>(T value,
                          T other,
                          [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IEquatable<T>?
        {
            ArgumentOutOfRangeException.ThrowIfEqual(value, other, paramName);
            return value;
        }

        public T GreaterThan<T>(T value,
                                T other,
                                [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IComparable<T>
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);
            return value;
        }

        public T GreaterThanOrEqual<T>(T value,
                                       T other,
                                       [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IComparable<T>
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);
            return value;
        }

        public T LessThan<T>(T value,
                             T other,
                             [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IComparable<T>
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);
            return value;
        }

        public T LessThanOrEqual<T>(T value,
                                    T other,
                                    [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IComparable<T>
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);
            return value;
        }

        public T NotEqual<T>(T value,
                             T other,
                             [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IEquatable<T>?
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);
            return value;
        }

        public T Negative<T>(T value,
                             [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : INumberBase<T>
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
            return value;
        }

        public T NegativeOrZero<T>(T value,
                                   [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : INumberBase<T>
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
            return value;
        }

        public T Zero<T>(T value,
                         [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : INumberBase<T>
        {
            ArgumentOutOfRangeException.ThrowIfZero(value, paramName);
            return value;
        }

#pragma warning restore CA1822 // Mark members as static

    }
}