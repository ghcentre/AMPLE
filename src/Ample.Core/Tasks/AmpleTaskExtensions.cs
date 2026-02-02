namespace Ample.Core.Tasks;

public static class AmpleTaskExtensions
{
    extension(Task)
    {
        /// <summary>
        /// Waits asynchronously for the specified timeout period unless the cancellation token is signaled, and indicates
        /// whether the delay completed without cancellation.
        /// </summary>
        /// <param name="timeout">The amount of time to wait before completing the delay. Must be a non-negative time span.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to signal cancellation before
        /// the timeout elapses.</param>
        /// <returns>A <see cref="Task"/> that completes with <see langword="true"/> if the timeout elapsed without cancellation;
        /// otherwise, <see langword="false"/> if the cancellation token was signaled before the timeout, -or-,
        /// the <paramref name="cancellationToken"/> source has been disposed.</returns>
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
                    try
                    {
                        bool signaled = cancellationToken.WaitHandle.WaitOne(timeout);
                        tcs.SetResult(!signaled);
                    }
                    catch (ObjectDisposedException)
                    {
                        //
                        // if the cancellation token source has been disposed, we consider the delay was canceled
                        //
                        tcs.SetResult(false);
                    }
                },
                CancellationToken.None);

            return tcs.Task;
        }
    }
}
