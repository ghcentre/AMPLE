namespace Ample.Core.GuardClauses;

public static class Guard
{
    public static IGuard Against { get; } = new GuardClauseImplementation();

    private class GuardClauseImplementation : IGuard
    {
    }
}
