using UnityEngine;

public class Mon001_SpawnState : EnemyStateBase
{
    public override EnemyStateName Name => EnemyStateName.Spawn;

    public override void OnEnterState() => Owner.ChangeState(EnemyStateName.Chase);
    public override void OnUpdateState() { }
    public override void OnExitState() { }
}
