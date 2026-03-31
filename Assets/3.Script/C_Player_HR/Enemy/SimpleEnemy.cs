using UnityEngine;
using UnityEngine.Pool;
using ShootingSpace.Core;
using System.Collections;

/// <summary>
/// 유니티 내장 풀링 시스템과 연동되며, 피격 피드백을 제공하는 적 클래스입니다.
/// </summary>
public class SimpleEnemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 30;
    private int _currentHealth;

    [Header("Feedback")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitEffectDuration = 0.1f;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Coroutine _hitEffectCoroutine;

    // --- 유니티 내장 풀 참조 ---
    private IObjectPool<SimpleEnemy> _pool;
    // -------------------------

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
    }

    /// <summary>
    /// 적에게 소속된 풀을 알려줍니다.
    /// </summary>
    public void SetPool(IObjectPool<SimpleEnemy> pool)
    {
        _pool = pool;
    }

    private void OnEnable()
    {
        _currentHealth = maxHealth;
        if (_spriteRenderer != null) _spriteRenderer.color = _originalColor;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        
        // 피격 피드백 실행
        if (_hitEffectCoroutine != null) StopCoroutine(_hitEffectCoroutine);
        _hitEffectCoroutine = StartCoroutine(HitEffectRoutine());

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitEffectRoutine()
    {
        if (_spriteRenderer == null) yield break;
        
        _spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitEffectDuration);
        _spriteRenderer.color = _originalColor;
    }

    private void Die()
    {
        // 사망 처리 (이펙트나 보상 생성 등 가능)
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
