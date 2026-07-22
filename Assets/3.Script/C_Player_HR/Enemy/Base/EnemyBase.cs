using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

/// <summary>
/// 적의 상태 종류를 정의하는 열거형입니다.
/// </summary>
public enum EnemyStateName
{
    Spawn,
    Idle,
    Chase,
    Attack,
    Die,
    Special
}

/// <summary>
/// 모든 적 개체의 기반이 되는 추상 클래스입니다. (BattleUnit 참조 구조 반영)
/// </summary>
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Title("기본 설정")]
    [Serializable] public class Stats
    {
        [LabelText("최대 체력")] public int maxHp;
        [LabelText("현재 체력")] public ClampedInt CurrentHp;
        [ShowInInspector, LabelText("현재 체력 (인스펙터)"), ReadOnly] 
        public int CurrentHpValue => CurrentHp?.Current ?? 0;
        [LabelText("이동 속도")] public float moveSpeed;
        [LabelText("회전 속도")] public float rotationSpeed;
    }
    [Serializable] public class Visuals
    {
        [field: SerializeField, LabelText("스프라이트 렌더러")] public SpriteRenderer SpriteRenderer { get; private set; }
        [field: SerializeField, LabelText("공격 인디케이터")] public EnemyIndicator Indicator { get; private set; }
        [field: SerializeField, LabelText("피격 효과")] public EnemyHitEffect HitEffect { get; private set; }
    }
    
    public Stats stats;
    public Visuals visuals;
    
    //---------------------------------------------------------------------------------------
    [TitleGroup("상태 머신", "하이러키(자식)에 상태 오브젝트들을 배치하세요.")]
    [SerializeField] private EnemyStateBase[] stateObjects;
    private readonly Dictionary<EnemyStateName, EnemyStateBase> statesDic = new Dictionary<EnemyStateName, EnemyStateBase>();
    
    [ShowInInspector, LabelText("현재 상태"), ReadOnly]
    public EnemyStateBase CurrentState { get; private set; }
    
    [SerializeField] private AttackStrategyBase attackStrategy; 
    public AttackStrategyBase AttackStrategy => attackStrategy;
    public float AttackRange => attackStrategy != null ? attackStrategy.attackRange : 0f;
    
    //---------------------------------------------------------------------------------------
    [Title("충돌 설정")]
    public Collider2D eCollider;
    [LabelText("충돌 데미지"), Tooltip("몬스터 몸에 부딪혔을 때의 데미지")]
    public int bumpDamage = 5;
    [LabelText("충돌 쿨타임"), SuffixLabel("초"), Tooltip("충돌 데미지 발생 후 재사용 대기 시간")]
    public float bumpCooldown = 1f;
    
    private float _lastBumpTime;

    // 특수 공격 중인지 확인하는 플래그
    public bool IsPerformingSpecialAttack { get; set; } = false;
    
    public bool IsDead { get; private set; } = false;
    
    protected virtual void Awake()
    {
        stats.CurrentHp = new ClampedInt(0, stats.maxHp, stats.maxHp)
        {
            Events =
            {
                OnMinReached = (prev, current) => { Die(); }
            }
        };
        
        statesDic.Clear();
        foreach (var state in stateObjects)
        {
            if (state == null) continue;
            statesDic.Add(state.Name, state);
            state.Initialize(this);
            state.gameObject.SetActive(false); // 초기화 시 비활성화
        }
    }
    
    protected virtual void Start()
    {
        IsDead = false;
        CurrentState = null;
        
        EnemyManager.Instance.RegisterEnemy(this);
        
        ChangeState(EnemyStateName.Spawn);
    }
    
    protected virtual void Update()
    {
        if (IsDead) return;
        CurrentState?.OnUpdateState();
    }
    
     protected virtual void OnDisable()
     {
         if (CurrentState != null)
         {
             CurrentState.OnExitState();
             CurrentState.gameObject.SetActive(false);
         }
         CurrentState = null;
 
         if (visuals.Indicator != null) visuals.Indicator.Hide();
 
         EnemyManager.Instance.UnregisterEnemy(this);
     }   
     
    /// <summary>
    /// 새로운 상태로 전환합니다. (참조 구조 반영)
    /// </summary>
    public void ChangeState(EnemyStateName nextStateName)
    {
        if (statesDic.TryGetValue(nextStateName, out var nextState))
        {
            if (CurrentState != null)
            {
                CurrentState.OnExitState();
                CurrentState.gameObject.SetActive(false);
            }

            CurrentState = nextState;
            CurrentState.gameObject.SetActive(true);
            CurrentState.OnEnterState();
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] {nextStateName} 상태를 찾을 수 없습니다!");
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 특수 공격 중이거나 쿨타임 중이면 무시
        if (IsPerformingSpecialAttack || Time.time < _lastBumpTime + bumpCooldown) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(bumpDamage);
                _lastBumpTime = Time.time;
            }
        }
    }
    
    public virtual void TakeDamage(int damage)
    {
        if (IsDead) return;
        stats.CurrentHp.Decrease(damage);

        if (visuals.HitEffect != null)
        {
            visuals.HitEffect.PlayHitEffect();
        }
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        ChangeState(EnemyStateName.Die);
    }
    
    public EnemyIndicator GetIndicator() => visuals.Indicator;
    public float GetMoveSpeed() => stats.moveSpeed;
    public SpriteRenderer GetSpriteRenderer() => visuals.SpriteRenderer;
}
