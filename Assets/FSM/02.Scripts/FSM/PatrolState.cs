public class PatrolState : IState
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
                monster.ChangeState(StateManager.GetState(StateManager.State.Trace));
            }
        }
        else
        {
            monster.Move();
        }
    }
}