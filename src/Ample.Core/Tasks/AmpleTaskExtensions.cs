namespace Ample.Core.Tasks;

public static class AmpleTaskExtensions
{
    public static Task<bool> DelaySafe(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();

        Task.Factory.StartNew(
            () =>
            {
                bool signaled = cancellationToken.WaitHandle.WaitOne(timeout);
                tcs.SetResult(!signaled);
            },
            CancellationToken.None);

        return tcs.Task;
    }
}
