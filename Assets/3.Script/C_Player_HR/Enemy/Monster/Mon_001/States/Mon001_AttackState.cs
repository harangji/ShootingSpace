using UnityEngine;

public class Mon001_AttackState : EnemyStateBase
{
    public override EnemyStateName Name => EnemyStateName.Attack;
    
    public override async void OnEnterState()
    {
        await owner.AttackStrategy.ExecuteAsync(owner);
        owner.ChangeState(EnemyStateName.Chase);
    }

    public override void OnUpdateState() { }

    public override void OnExitState() { }
}
