using UnityEngine;

/// <summary>
/// 플레이어의 현재 위치를 향해 직선으로 이동하는 가장 기본적인 적입니다.
/// 2D 환경에 맞춰 Transform을 사용하여 직접 이동합니다.
/// </summary>
public class FollowEnemy : EnemyBase
{
    protected void HandleMovement()
    {
        // 플레이어 방향으로의 벡터 계산 (2D)
        Vector2 currentPos = transform.position;
        Vector2 playerPos = GameManager.Instance.playerTransform.position;
        Vector2 direction = (playerPos - currentPos).normalized;
        
        // Transform을 이용한 2D 이동
        transform.Translate(direction * (stats.moveSpeed * Time.deltaTime), Space.World);

        // 2D 회전: Sprite의 위쪽(Up)이 진행 방향을 향하도록 설정
        if (direction != Vector2.zero)
        {
            transform.up = direction;
        }
    }

    /// <summary>
    /// 플레이어와 충돌했을 때의 처리 (Trigger 체크)
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryApplyBumpDamage(other);
    }
}
