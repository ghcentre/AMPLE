using Ample.Core.Tasks;

namespace Ample.Core.Tests.Tasks;

public class AmpleTaskExtensions_Tests
{
    [Fact]
    public async Task DelaySafe_NotCancelled_ReturnsTrue()
    {
        var timeout = TimeSpan.FromMilliseconds(100);
        using var cts = new CancellationTokenSource();

        bool result = await Task.DelaySafe(timeout, cts.Token);

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task DelaySafe_Cancelled_ReturnsFalse()
    {
        var timeout = TimeSpan.FromMilliseconds(100);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        bool result = await Task.DelaySafe(timeout, cts.Token);

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task DelaySafe_Cancelled_DoesNotThrow()
    {
        var timeout = TimeSpan.FromMilliseconds(100);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Should.NotThrowAsync(async () => await Task.DelaySafe(timeout, cts.Token));
    }
}
