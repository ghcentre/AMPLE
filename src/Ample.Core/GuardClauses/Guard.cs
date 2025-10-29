namespace Ample.Core.GuardClauses;

/// <summary>
/// Acts as a <see langword="static"/> placeholder for the <see cref="IGuard"/> implementation.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Provides an entry point to all guard clause extensions.
    /// </summary>
    public static IGuard Against { get; } = new GuardClauseImplementation();

    /// <summary>
    /// Private implementation of the <see cref="IGuard"/> interface.
    /// </summary>
    private class GuardClauseImplementation : IGuard;
}
