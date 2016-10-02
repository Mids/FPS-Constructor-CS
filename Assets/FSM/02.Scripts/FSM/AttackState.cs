public class AttackState : IState
{
    public void Run(MonsterCtrl monster)
    {
        if (monster.Detect())
        {
            if (monster.IsAttackable())
            {
                monster.Attack();
            }
            else
            {
                monster.ChangeState(StateManager.GetState(StateManager.State.Trace));
            }
        }
        else
        {
            monster.ChangeState(StateManager.GetState(StateManager.State.Patrol));
        }
    }
}