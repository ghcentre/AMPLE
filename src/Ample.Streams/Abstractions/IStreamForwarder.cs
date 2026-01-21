namespace Ample.Streams.Abstractions;

public interface IStreamForwarder
{
    Task<long> ForwardBidirectionalAsync(
        Stream clientStream,
        Stream serverStream,
        byte[] clientBuffer,
        byte[] serverBuffer,
        IInspector inspector,
        CancellationToken cancellationToken);
}
