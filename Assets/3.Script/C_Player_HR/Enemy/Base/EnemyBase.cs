using UnityEngine;
using UnityEngine.Pool;
using ShootingSpace.Core;
using System.Collections;
using System;
using Sirenix.OdinInspector;

/// <summary>
/// 모든 적 개체의 기반이 되는 추상 클래스입니다.
/// </summary>
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [TitleGroup("기본 능력치", "적의 생존과 이동에 관한 스탯입니다.", alignment: TitleAlignments.Left)]
    [LabelText("최대 체력"), GUIColor(1, 0.5f, 0.5f)]
    [SerializeField] protected int maxHealth = 30;

    [LabelText("이동 속도")]
    [SerializeField] protected float moveSpeed = 1f;

    [ShowInInspector, LabelText("현재 체력"), ReadOnly, ProgressBar(0, "maxHealth", ColorGetter = "GetHealthColor")]
    protected int currentHealth;

    [TitleGroup("시각적 피드백")]
    [LabelText("피격 색상")]
    [SerializeField] protected Color hitColor = Color.red;
    [LabelText("피격 효과 시간")]
    [SerializeField] protected float hitEffectDuration = 0.1f;
    
    [LabelText("스프라이트 렌더러")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    
    [LabelText("공격 인디케이터")]
    [SerializeField] protected EnemyIndicator indicator;

    protected Color originalColor;
    private Coroutine _hitEffectCoroutine;

    protected Transform playerTransform;
    [ShowInInspector, LabelText("사망 여부"), ReadOnly]
    protected bool isDead = false;

    private Color GetHealthColor(int value) => Color.Lerp(Color.red, Color.green, (float)value / maxHealth);

    protected virtual void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
        EnsurePlayerReference();

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.RegisterEnemy(this);
    }

    protected virtual void OnDisable()
    {
        if (indicator != null) indicator.Hide();

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.UnregisterEnemy(this);
    }

    protected void EnsurePlayerReference()
    {
        if (playerTransform == null && GameManager.Instance != null)
            playerTransform = GameManager.Instance.playerTransform;
    }

    protected virtual void Update()
    {
        if (isDead) return;
        HandleMovement();
    }

    protected abstract void HandleMovement();

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        
        if (_hitEffectCoroutine != null) StopCoroutine(_hitEffectCoroutine);
        _hitEffectCoroutine = StartCoroutine(HitEffectRoutine());

        if (currentHealth <= 0) Die();
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

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.ReportEnemyKilled(this);

        ReturnToPool();
    }

    protected void ReturnToPool()
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.ReleaseEnemy(this);
        else
            Destroy(gameObject);
    }

    public EnemyIndicator GetIndicator() => indicator;
    public Transform GetPlayerTransform() => playerTransform;
    public float GetMoveSpeed() => moveSpeed;
    public void SetMoveSpeed(float speed) => moveSpeed = speed;
    public SpriteRenderer GetSpriteRenderer() => spriteRenderer;
}
