using UnityEngine;

public class Mon001_AttackState : EnemyStateBase
{
    public override EnemyStateName Name => EnemyStateName.Attack;
    
    public override async void OnEnterState()
    {
        await Owner.AttackStrategy.ExecuteAsync(Owner);
        Owner.ChangeState(EnemyStateName.Chase);
    }

    public override void OnUpdateState() { }

    public override void OnExitState() { }
}
