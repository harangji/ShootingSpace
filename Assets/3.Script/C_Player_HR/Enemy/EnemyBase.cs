using UnityEngine;
using UnityEngine.Pool;
using ShootingSpace.Core;
using System.Collections;
using System;

/// <summary>
/// 모든 적 개체의 기반이 되는 추상 클래스입니다.
/// </summary>
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    public static event Action<EnemyBase> OnEnemyKilled;

    [Header("Base Stats")]
    [SerializeField] protected int maxHealth = 30;
    [SerializeField] protected float moveSpeed = 3f;
    protected int currentHealth;

    [Header("Visual Feedback")]
    [SerializeField] protected Color hitColor = Color.red;
    [SerializeField] protected float hitEffectDuration = 0.1f;
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;
    private Coroutine _hitEffectCoroutine;

    protected Transform playerTransform;
    protected bool isDead = false;

    // --- 풀링 시스템 (컴포넌트 타입으로 추상화) ---
    private IObjectPool<GameObject> _pool;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        
        // 플레이어 찾기 (이름이나 태그 등으로 찾을 수 있음)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    public void SetPool(IObjectPool<GameObject> pool)
    {
        _pool = pool;
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }

    /// <summary>
    /// 매 프레임 실행될 로직 (주로 이동)
    /// </summary>
    protected virtual void Update()
    {
        if (isDead) return;
        HandleMovement();
    }

    /// <summary>
    /// 자식 클래스에서 구현할 이동 로직입니다.
    /// </summary>
    protected abstract void HandleMovement();

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        if (_hitEffectCoroutine != null) StopCoroutine(_hitEffectCoroutine);
        _hitEffectCoroutine = StartCoroutine(HitEffectRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected IEnumerator HitEffectRoutine()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitEffectDuration);
        spriteRenderer.color = originalColor;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        // 사망 이벤트 알림
        OnEnemyKilled?.Invoke(this);

        ReturnToPool();
    }

    protected void ReturnToPool()
    {
        if (_pool != null)
        {
            _pool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
