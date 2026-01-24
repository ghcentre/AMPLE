using Ample.Streams.Abstractions;

namespace Ample.Streams;

internal class DefaultInspector : IInspector
{
    public ValueTask<InspectionResult> InspectAsync(IInspectionChunk chunk)
    {
        // Default implementation simply allows all data through
        return ValueTask.FromResult(InspectionResult.Send);
    }
}