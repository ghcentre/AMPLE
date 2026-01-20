namespace Ample.Streams.Abstractions;

public interface IInspector
{
    ValueTask<InspectionResult> InspectAsync(IInspectionChunk chunk);
}
