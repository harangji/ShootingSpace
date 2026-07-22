using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 적중 시 주변의 원형 범위 내의 모든 적에게 데미지를 주는 폭발형 탄환입니다.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet_02 : MonoBehaviour, IPoolable<Bullet_02>
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float explosionRadius = 1.5f; // 폭발 범위
    [SerializeField] private float rotationSpeed = 270f;  // 자전 속도 (도/초)

    private BulletData _data;
    private bool _isInitialized = false;

    // --- 유니티 내장 풀 참조 ---
    private IObjectPool<Bullet_02> _pool;
    // -------------------------

    private float _lifeTime = 5f;
    private float _spawnTime;

    /// <summary>
    /// 탄환에게 소속된 풀을 알려줍니다.
    /// </summary>
    public void SetPool(IObjectPool<Bullet_02> pool)
    {
        _pool = pool;
    }

    /// <summary>
    /// 탄환을 초기화하고 수명 주기를 설정합니다.
    /// </summary>
    /// <param name="data">탄환 생성 데이터</param>
    public void Init(BulletData data)
    {
        _data = data;
        _spawnTime = Time.time;
        
        // 전달받은 데이터의 방향을 적용!
        transform.up = data.direction;

        // 외형 및 크기 적용
        if (TryGetComponent<SpriteRenderer>(out var sr) && data.bulletSprite != null)
        {
            sr.sprite = data.bulletSprite;
        }
        transform.localScale = data.scale;

        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        // 수명 체크
        if (Time.time >= _spawnTime + _lifeTime)
        {
            ReturnToPool();
            return;
        }

        // 이동 처리 (발사 시 정해진 방향으로 직선 이동)
        transform.position += (Vector3)_data.direction * (_data.speed * Time.deltaTime);

        // 자전 처리 (이동 방향에 상관없이 스스로 회전)
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 반납 처리 중인 경우 무시 (중복 실행 방지)
        if (!_isInitialized) return;

        // 적 레이어와 충돌했는지 확인
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            Explode();

            // 폭발 탄환은 보통 적중 시 소멸하지만, 관통 수치가 있다면 유지할 수도 있습니다.
            if (_data.pierceCount <= 0)
            {
                ReturnToPool();
            }
            else
            {
                _data.pierceCount--;
            }
        }
    }

    /// <summary>
    /// 원형 범위 내의 모든 적에게 데미지를 줍니다.
    /// </summary>
    private void Explode()
    {
        // 원형 범위 내의 모든 Collider2D를 스캔
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);

        foreach (var enemyCollider in hitEnemies)
        {
            if (enemyCollider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(_data.damage);
                Debug.Log($"폭발 데미지 적용 : {enemyCollider.name}에게 {_data.damage}의 피해!");
            }
        }
    }

    private void ReturnToPool()
    {
        // 이미 반납 처리된 경우 중복 실행 방지
        if (!_isInitialized) return;

        _isInitialized = false;
        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 에디터에서 폭발 범위를 시각적으로 확인하기 위한 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
