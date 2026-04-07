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
    [Header("Base Stats")]
    [SerializeField] protected int maxHealth = 30;
    [SerializeField] protected float moveSpeed = 3f;
    protected int currentHealth;

    [Header("Visual Feedback")]
    [SerializeField] protected Color hitColor = Color.red;
    [SerializeField] protected float hitEffectDuration = 0.1f;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected Color originalColor;
    private Coroutine _hitEffectCoroutine;

    protected Transform playerTransform;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
        
        // 활성화될 때 플레이어 참조를 확실히 가져옴
        EnsurePlayerReference();

        // 중앙 매니저에 등록
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterEnemy(this);
        }
    }

    protected virtual void OnDisable()
    {
        // 비활성화될 때 중앙 매니저에서 등록 해제
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.UnregisterEnemy(this);
        }
    }

    /// <summary>
    /// GameManager로부터 플레이어 참조를 안전하게 가져옵니다.
    /// </summary>
    protected void EnsurePlayerReference()
    {
        if (playerTransform == null && GameManager.Instance != null)
        {
            playerTransform = GameManager.Instance.playerTransform;
        }
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

        // 중앙 매니저에 사망 보고
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.ReportEnemyKilled(this);
        }

        ReturnToPool();
    }

    protected void ReturnToPool()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.ReleaseEnemy(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
