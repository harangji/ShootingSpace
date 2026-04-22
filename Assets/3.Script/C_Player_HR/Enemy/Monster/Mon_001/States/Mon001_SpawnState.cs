using UnityEngine;

public class Mon001_SpawnState : EnemyStateBase
{
    public override EnemyStateName Name => EnemyStateName.Spawn;

    public override void OnEnterState() => owner.ChangeState(EnemyStateName.Chase);
    public override void OnUpdateState() { }
    public override void OnExitState() { }
}
