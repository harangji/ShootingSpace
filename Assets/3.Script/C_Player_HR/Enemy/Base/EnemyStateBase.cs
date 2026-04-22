using UnityEngine;

/// <summary>
/// 모든 몬스터 상태의 기반이 되는 추상 클래스입니다. (참조 구조 반영)
/// </summary>
public abstract class EnemyStateBase : MonoBehaviour
{
    protected EnemyBase owner;

    public abstract EnemyStateName Name { get; }

    public virtual void Initialize(EnemyBase enemy)
    {
        owner = enemy;
    }

    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnExitState();
}
