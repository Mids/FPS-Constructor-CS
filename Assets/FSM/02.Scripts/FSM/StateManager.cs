public static class StateManager
{
    public enum State
    {
        Patrol,
        Trace,
        Attack
    }

    private static readonly IState[] StatesList = {new PatrolState(), new TraceState(), new AttackState()};

    public static IState GetState(State state)
    {
        return StatesList[(int) state];
    }
}