using Ample.Core.GuardClauses;
using Ample.Streams.Abstractions;

namespace Ample.Streams.Exceptions;

public class ForwardException(
        Operation operation,
        Side side,
        Exception innerException)
    : InvalidOperationException(
        $"{side} {operation} failed: {Guard.Against.Null(innerException).Message}",
        innerException);
