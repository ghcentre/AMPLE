namespace Ample.Streams.Abstractions;

internal interface IStreamForwarder
{
    Task<long> ForwardBidirectionalAsync(
        Stream clientStream,
        Stream serverStream,
        byte[] clientBuffer,
        byte[] serverBuffer,
        IInspector inspector,
        CancellationToken cancellationToken);
}
