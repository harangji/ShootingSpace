using UnityEngine;

public class Mon001_ChaseState : EnemyStateBase
{
    public override EnemyStateName Name => EnemyStateName.Chase;

    public override void OnEnterState() { }

    public override void OnUpdateState()
    {
        Transform player = GameManager.Instance.playerTransform;

        // 플레이어를 향해 계속 이동
        Vector2 direction = ((Vector2)player.position - (Vector2)owner.transform.position).normalized;
        owner.transform.Translate(direction * (owner.GetMoveSpeed() * Time.deltaTime), Space.World);

        if (direction != Vector2.zero)
        {
            owner.transform.up = direction;
        }

        // 사거리 안에 있고 전략이 사용 가능한 경우에만 공격 상태로 전환
        float distance = Vector2.Distance(owner.transform.position, player.position);
        if (distance <= owner.AttackRange && owner.AttackStrategy.CanExecute())
        {
            owner.ChangeState(EnemyStateName.Attack);
        }
    }

    public override void OnExitState() { }
}
