using UnityEngine;
using UnityEngine.Pool; // 유니티 내장 풀링 시스템 라이브러리 추가!
using ShootingSpace.Core;


/// <summary>
/// 가장 가까운 적을 추적하며 이동하는 유도 탄환의 동작을 정의하고,
/// 유니티 내장 풀과 연동되는 컴포넌트입니다.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet_03 : MonoBehaviour, IPoolable<Bullet_03>
{
    [SerializeField] private LayerMask m_enemyLayer;
    [SerializeField] private float m_rotationSpeed = 360f; // 초당 회전 각도 (유도 성능)

    private BulletData m_data;
    private bool m_isInitialized = false;
    private Transform m_targetEnemy; // 추적할 적의 트랜스폼

    // --- 유니티 내장 풀 참조 ---
    private IObjectPool<Bullet_03> m_pool;
    // -------------------------

    private float m_lifeTime = 5f;
    private float m_spawnTime;

    /// <summary>
    /// 탄환에게 소속된 풀을 알려줍니다.
    /// </summary>
    public void SetPool(IObjectPool<Bullet_03> pool)
    {
        m_pool = pool;
    }

    /// <summary>
    /// 탄환을 초기화하고 수명 주기를 설정합니다.
    /// </summary>
    /// <param name="data">탄환 생성 데이터</param>
    public void Init(BulletData data)
    {
        m_data = data;
        m_spawnTime = Time.time;
        
        // 외형 및 크기 적용
        if (TryGetComponent<SpriteRenderer>(out var sr) && data.bulletSprite != null)
        {
            sr.sprite = data.bulletSprite;
        }
        transform.localScale = data.scale;

        // 초기화 시 가장 가까운 적을 찾습니다.
        FindNearestEnemy();

        m_isInitialized = true;
    }

    private void OnDisable()
    {
        // 풀로 돌아갈 때 타겟을 초기화합니다.
        m_targetEnemy = null;
    }

    private void Update()
    {
        if (!m_isInitialized) return;

        // 수명 체크
        if (Time.time >= m_spawnTime + m_lifeTime)
        {
            ReturnToPool();
            return;
        }

        // 1. 유도 대상이 없다면, 다시 찾거나 직진합니다.
        if (m_targetEnemy == null || !m_targetEnemy.gameObject.activeInHierarchy)
        {
            FindNearestEnemy(); // 대상을 잃었거나 사라졌다면 다시 찾습니다.
            if (m_targetEnemy == null)
            {
                // 대상을 찾지 못하면 직진 (Bullet_01과 동일)
                transform.position += transform.up * (m_data.speed * Time.deltaTime);
                return;
            }
        }

        // 2. 대상이 있다면 추적 및 회전 처리
        Vector2 directionToTarget = (m_targetEnemy.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f; // Unity 2D Sprite 기본 방향 보정
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // 부드러운 회전
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);

        // 3. 이동 처리 (항상 현재 transform.up 방향으로 이동)
        transform.position += transform.up * (m_data.speed * Time.deltaTime);
    }

    private void FindNearestEnemy()
    {
        // EnemyManager에서 관리하는 활성화된 적 리스트를 활용
        // 'FindObjectsByType' 방식은 성능에 영향을 줄 수 있으므로 이 방법을 사용합니다.
        float minDistance = float.MaxValue;
        Transform nearestEnemy = null;

        if (EnemyManager.Instance != null)
        {
            foreach (EnemyBase enemy in EnemyManager.Instance.ActiveEnemies)
            {
                // EnemyManager.ActiveEnemies는 이미 활성화된 적만 포함하므로 activeInHierarchy 체크는 불필요
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }
        }
        else
        {
            // EnemyManager가 없을 경우 (예: 에디터에서 단독 실행 시)를 위한 폴백 또는 경고
            Debug.LogWarning("EnemyManager.Instance가 없습니다. 가장 가까운 적을 찾을 수 없습니다.");
        }
        m_targetEnemy = nearestEnemy;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 반납 처리 중인 경우 무시 (중복 실행 방지)
        if (!m_isInitialized) return;

        if (((1 << other.gameObject.layer) & m_enemyLayer) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(m_data.damage);
                Debug.Log($"데미지 : {m_data.damage}");
            }

            if (m_data.pierceCount <= 0)
            {
                ReturnToPool();
            }
            else
            {
                m_data.pierceCount--;
            }
        }
    }

    private void ReturnToPool()
    {
        // 이미 반납 처리된 경우 중복 실행 방지
        if (!m_isInitialized) return;

        m_isInitialized = false;
        if (m_pool != null)
        {
            m_pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
