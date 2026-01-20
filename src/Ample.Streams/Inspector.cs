using Ample.Streams.Abstractions;

namespace Ample.Streams;

public static class Inspector
{
    public static readonly IInspector Default = new DefaultInspector();
}
