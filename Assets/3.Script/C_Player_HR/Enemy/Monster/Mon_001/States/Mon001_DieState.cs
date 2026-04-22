using UnityEngine;

public class Mon001_DieState : EnemyStateBase
{
    public override EnemyStateName Name => EnemyStateName.Die;

    public override void OnEnterState()
    {
        EnemyManager.Instance.ReportEnemyKilled(owner);
        EnemyManager.Instance.ReleaseEnemy(owner);
    }
    public override void OnUpdateState() { }
    public override void OnExitState() { }
}
