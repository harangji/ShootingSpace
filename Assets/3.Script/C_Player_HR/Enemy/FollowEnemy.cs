using UnityEngine;

/// <summary>
/// 플레이어의 현재 위치를 향해 직선으로 이동하는 가장 기본적인 적입니다.
/// Rigidbody2D를 사용하여 물리 기반으로 이동합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class FollowEnemy : EnemyBase
{
    private Rigidbody2D _rb;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        
        // 2D 게임이므로 중력 영향을 받지 않도록 설정 (필요시 Inspector에서 조정 가능)
        _rb.gravityScale = 0f;
    }

    protected override void HandleMovement()
    {
        if (playerTransform == null || _rb == null) return;

        // 플레이어 방향으로의 벡터 계산
        Vector2 currentPos = _rb.position;
        Vector2 playerPos = playerTransform.position;
        Vector2 direction = (playerPos - currentPos).normalized;
        
        // Rigidbody2D의 velocity를 직접 제어하여 이동
        _rb.linearVelocity = direction * moveSpeed;

        // 진행 방향을 바라보게 회전
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            _rb.rotation = angle;
        }
    }

    protected override void Update()
    {
        if (isDead)
        {
            if (_rb != null) _rb.linearVelocity = Vector2.zero;
            return;
        }
        
        HandleMovement();
    }

    /// <summary>
    /// 플레이어와 충돌했을 때 데미지를 주거나 밀려나는 등의 처리를 할 수 있습니다.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Enemy] 플레이어와 충돌!");
        }
    }
}
