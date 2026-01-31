namespace Ample.Streams.Abstractions;

public interface IStreamForwarder
{
    long ClientToServerBytesTransferred { get; }
    
    long ServerToClientBytesTransferred { get; }

    Task ForwardBidirectionalAsync(
        string sessionId,
        Stream clientStream,
        Stream serverStream,
        byte[] clientBuffer,
        byte[] serverBuffer,
        TimeSpan timeout,
        IInspector inspector,
        CancellationToken cancellationToken);
}
