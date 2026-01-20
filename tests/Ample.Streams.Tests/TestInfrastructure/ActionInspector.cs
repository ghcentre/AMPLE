using Ample.Streams.Abstractions;

namespace Ample.Streams.Tests.TestInfrastructure;

internal class ActionInspector(Func<IInspectionChunk, ValueTask<InspectionResult>> action) : IInspector
{
    public async ValueTask<InspectionResult> InspectAsync(IInspectionChunk chunk)
    {
        return await action(chunk);
    }
}
