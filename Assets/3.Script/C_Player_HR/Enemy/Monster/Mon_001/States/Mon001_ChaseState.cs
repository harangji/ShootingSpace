using UnityEngine;

public class Mon001_ChaseState : EnemyStateBase
{
    public override EnemyStateName Name => EnemyStateName.Chase;

    private Transform _player;
    private Vector2 _direction;
    private float _distance;
    private float _stoppingDistance;

    public override void OnEnterState() 
    {
        _player = GameManager.Instance.playerTransform;
    }

    public override void OnUpdateState()
    {
        if (!_player) return;

        _distance = Vector2.Distance(Owner.transform.position, _player.position);
        _direction = (_player.position - Owner.transform.position).normalized;

        // 회전은 사거리와 관계없이 계속 수행
        Rotate(_direction);

        // 공격 사거리 내라면 공격 실행 및 이동 중지
        if (_distance <= Owner.AttackRange)
        {
            if (Owner.AttackStrategy.CanExecute())
            {
                Owner.ChangeState(EnemyStateName.Attack);
            }
            return;
        }

        // 사거리 밖일 때만 이동
        Move(_direction);
    }

    public override void OnExitState() { }
    
    private void Move(Vector2 direction)
    {
        Owner.transform.Translate(direction * (Owner.GetMoveSpeed() * Time.deltaTime), Space.World);
    }
    
    private void Rotate(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            Owner.transform.up = direction;
        }
    }
}
