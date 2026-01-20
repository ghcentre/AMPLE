namespace Ample.Streams.Exceptions;

public class ForwardException(
        Operation operation,
        Side side,
        Exception innerException)
    : InvalidOperationException(
        $"{side} {operation} failed.",
        innerException);
