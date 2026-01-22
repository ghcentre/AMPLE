namespace Ample.Core.Tasks;

public static class AmpleTaskExtensions
{
    /// <summary>
    /// Waits asynchronously for the specified timeout period unless the cancellation token is signaled, and indicates
    /// whether the delay completed without cancellation.
    /// </summary>
    /// <param name="timeout">The amount of time to wait before completing the delay. Must be a non-negative time span.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation before
    /// the timeout elapses.</param>
    /// <returns>A <see cref="Task"/> that completes with <see langword="true"/> if the timeout elapsed without cancellation;
    /// otherwise, <see langword="false"/> if the cancellation token was signaled before the timeout.</returns>
    /// <remarks>
    /// This method does not throw if the cancellation token is signaled; instead, it returns <see langword="false"/>.
    /// The returned task always completes successfully.
    /// </remarks>
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
