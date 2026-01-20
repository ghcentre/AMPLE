namespace Ample.Streams.Exceptions;

public class BufferOverflowException(Exception innerException) : InvalidOperationException("Buffer overflow.", innerException);
