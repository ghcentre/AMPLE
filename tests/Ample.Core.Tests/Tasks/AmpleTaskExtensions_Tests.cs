using Ample.Core.Tasks;

namespace Ample.Core.Tests.Tasks;

public class AmpleTaskExtensions_Tests
{
    [Fact]
    public async Task DelaySafe_NotCanceled_ReturnsTrue()
    {
        var timeout = TimeSpan.FromMilliseconds(100);
        using var cts = new CancellationTokenSource();

        bool result = await AmpleTaskExtensions.DelaySafe(timeout, cts.Token);

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task DelaySafe_Canceled_ReturnsFalse()
    {
        var timeout = TimeSpan.FromMilliseconds(100);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        bool result = await AmpleTaskExtensions.DelaySafe(timeout, cts.Token);

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task DelaySafe_Canceled_DoesNotThrow()
    {
        var timeout = TimeSpan.FromMilliseconds(100);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Should.NotThrowAsync(async () => await AmpleTaskExtensions.DelaySafe(timeout, cts.Token));
    }
}
