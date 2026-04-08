using UnityEngine;
using UnityEngine.Pool;
using ShootingSpace.Core;
using System.Collections.Generic; // List 사용을 위해 추가

using DG.Tweening; // DOTween 사용

/// <summary>
/// 발사 당시 랜덤한 적의 위치로 날아가며, 도착 시 5배 커져 3초간 머무르다가 파괴되는 탄환의 동작을 정의합니다.
/// 비행 중에는 물리적 상호작용은 없으나, 적에게 데미지를 줄 수 있습니다.
/// 도착 후 확대된 상태에서는 지속적으로 데미지를 입힙니다.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet_04 : MonoBehaviour, IPoolable<Bullet_04>
{
    [SerializeField] private LayerMask m_enemyLayer;
    [SerializeField] private float m_flightSpeed = 10f; // 초기 비행 속도
    [SerializeField] private float m_scaleMultiplier = 5f; // 도착 후 커지는 배율
    [SerializeField] private float m_stayDuration = 3f; // 도착 후 머무는 시간
    [SerializeField] private float m_arrivalThreshold = 0.5f; // 대상 위치에 도착했다고 판단할 거리
    [SerializeField] private float m_damageTickInterval = 0.5f; // 확대 상태에서 데미지 주기 간격

    private BulletData m_data; // Bullet_01/Bullet_03과 동일한 BulletData 사용 가정
    private bool m_isInitialized = false;
    private IObjectPool<Bullet_04> m_pool;
    private Vector3 m_targetPosition;
    private bool m_hasArrived = false;
    private float m_lastDamageTickTime;
    private float m_initialScale; // 원래 크기 저장
    private CircleCollider2D m_circleCollider; // 충돌체 캐싱

    /// <summary>
    /// 탄환에게 소속된 풀을 알려줍니다.
    /// </summary>
    public void SetPool(IObjectPool<Bullet_04> pool)
    {
        m_pool = pool;
    }

    /// <summary>
    /// 탄환을 초기화하고 수명 주기를 설정합니다.
    /// </summary>
    /// <param name="data">탄환 생성 데이터</param>
    public void Init(BulletData data)
    {
        if (m_circleCollider == null)
        {
            m_circleCollider = GetComponent<CircleCollider2D>();
        }

        m_data = data;
        // 버그 수정: m_initialScale을 BulletData에서 받은 스케일로 설정합니다.
        // 이렇게 해야 증강 효과가 반영된 정확한 초기 스케일을 가집니다.
        m_initialScale = data.scale.x; 
        transform.localScale = data.scale; // BulletData의 스케일을 직접 적용합니다.
                                           // m_initialScale은 이 값을 기억하는 용도로만 사용합니다.
        m_hasArrived = false;
        m_isInitialized = true;
        m_lastDamageTickTime = Time.time;

        // 랜덤 적 위치 찾기
        Vector3? randomEnemyPos = FindRandomEnemyPosition();
        if (randomEnemyPos.HasValue)
        {
            m_targetPosition = randomEnemyPos.Value;
            // 타겟 방향으로 회전 (SpriteRenderer의 up이 위를 향하도록)
            Vector3 direction = (m_targetPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // 적이 없으면 풀로 반환 (또는 직진)
            Debug.LogWarning("Bullet_04: No enemies found to target. Returning to pool.");
            ReturnToPool();
            return;
        }
    }

    private void Update()
    {
        if (!m_isInitialized) return;

        if (!m_hasArrived)
        {
            // 비행 중
            transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, m_flightSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, m_targetPosition) < m_arrivalThreshold)
            {
                // 도착 처리
                m_hasArrived = true;
                // 크기 확대 애니메이션
                transform.DOScale(m_initialScale * m_scaleMultiplier, 0.5f)
                    .SetEase(Ease.OutBack); // 부드러운 확대 애니메이션
                
                // 지정된 시간 후 풀로 반환
                DOVirtual.DelayedCall(m_stayDuration, ReturnToPool);
            }
        }
        else
        {
            // 도착 후 확대 상태
            // 지속적인 데미지 처리
            if (Time.time >= m_lastDamageTickTime + m_damageTickInterval)
            {
                DealContinuousDamage();
                m_lastDamageTickTime = Time.time;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_isInitialized) return;

        // 날아가는 중에는 데미지를 주지 않고, 도착하여 확대된 상태에서만 데미지 처리
        if (m_hasArrived) // m_hasArrived가 true일 때만 데미지를 처리합니다.
        {
            if (((1 << other.gameObject.layer) & m_enemyLayer) != 0)
            {
                if (other.TryGetComponent<IDamageable>(out var target))
                {
                    target.TakeDamage(m_data.damage);
                    // 총알은 파괴되지 않음 (ReturnToPool 호출 안함)
                    Debug.Log($"Bullet_04 데미지 적용: {m_data.damage}");
                }
            }
        }
    }

    private void DealContinuousDamage()
    {
        // 확대된 충돌체 영역 내의 모든 적에게 데미지 적용
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, m_circleCollider.radius * transform.localScale.x, m_enemyLayer);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(m_data.damage);
                Debug.Log($"Bullet_04 지속 데미지 적용: {m_data.damage}");
            }
        }
    }

    private Vector3? FindRandomEnemyPosition()
    {
        // EnemyManager에서 관리하는 활성화된 적 리스트를 활용
        // 'FindObjectsByType' 방식은 성능에 영향을 줄 수 있으므로 이 방법을 사용합니다.
        List<Transform> activeEnemyTransforms = new List<Transform>();

        if (EnemyManager.Instance != null && EnemyManager.Instance.ActiveEnemies.Count > 0)
        {
            foreach (EnemyBase enemy in EnemyManager.Instance.ActiveEnemies)
            {
                activeEnemyTransforms.Add(enemy.transform);
            }

            if (activeEnemyTransforms.Count > 0)
            {
                int randomIndex = Random.Range(0, activeEnemyTransforms.Count);
                return activeEnemyTransforms[randomIndex].position;
            }
        }
        else
        {
            Debug.LogWarning("EnemyManager.Instance가 없거나 활성화된 적이 없습니다. 랜덤 적 위치를 찾을 수 없습니다.");
        }
        return null; // 적을 찾지 못함
    }

    private void ReturnToPool()
    {
        // DOTween 애니메이션이 실행 중일 수 있으므로 안전하게 중지
        transform.DOKill();
        
        if (!m_isInitialized) return;

        m_isInitialized = false;

        // 버그 수정: 풀로 반환하기 전에 크기를 원래 상태로 재설정합니다.
        // 그렇지 않으면 다음번 풀에서 꺼낼 때 이전 사용에서 확대된 크기가 유지될 수 있습니다.
        transform.localScale = Vector3.one * m_initialScale; // 원래 초기 크기로 재설정

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
