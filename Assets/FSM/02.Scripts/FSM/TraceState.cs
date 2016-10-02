public class TraceState : IState
{
    public void Run(MonsterCtrl monster)
    {
        if (monster.Detect())
        {
            if (monster.IsAttackable())
            {
                monster.ChangeState(StateManager.GetState(StateManager.State.Attack));
            }
            else
            {
                monster.Move();
            }
        }
        else
        {
            monster.ChangeState(StateManager.GetState(StateManager.State.Patrol));
        }
    }
}