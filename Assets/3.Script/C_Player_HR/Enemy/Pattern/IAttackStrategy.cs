using UnityEngine;

/// <summary>
/// 모든 몬스터 객체가 공유할 수 있는 공격 전략 인터페이스입니다.
/// </summary>
public interface IAttackStrategy
{
    void Execute(EnemyBase enemy);
}
