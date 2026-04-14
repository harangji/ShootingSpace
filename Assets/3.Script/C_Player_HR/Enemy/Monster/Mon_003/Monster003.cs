using UnityEngine;
using Sirenix.OdinInspector;

public class Monster003 : EnemyBase
{
    [Title("중장갑 유닛 사거리 설정")]
    [LabelText("주력 공격 전략"), Required]
    [SerializeField] private AttackStrategyBase primaryStrategy;
    
    [InfoBox("@primaryStrategy.description", VisibleIf = "primaryStrategy != null")]
    [Space(5)]

    [LabelText("주력 공격 사거리")]
    [SerializeField] private float primaryAttackRange = 10f;

    [LabelText("최후의 자폭 전략"), Required]
    [SerializeField] private AttackStrategyBase panicStrategy;

    [InfoBox("@panicStrategy.description", VisibleIf = "panicStrategy != null")]
    [Space(10)]

    [LabelText("자폭 발동 조건"), Tooltip("이 거리 이하로 근접하거나 체력이 낮으면 해당 전략을 사용합니다.")]
    [SerializeField] private float panicThreshold = 3f;

    [Title("실시간 상태")]
    [LabelText("현재 활성 전략"), ReadOnly]
    [ShowInInspector] private string CurrentActiveStrategy => _currentStrategy?.name ?? "None";

    private AttackStrategyBase _currentStrategy;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Update()
    {
        base.Update();
        if (isDead) return;

        HandleStrategySelection();

        if (_currentStrategy != null)
        {
            _currentStrategy.Execute(this);
        }
    }

    private void HandleStrategySelection()
    {
        if (playerTransform == null) return;
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (currentHealth < maxHealth * 0.25f || distance <= panicThreshold)
        {
            _currentStrategy = panicStrategy;
        }
        else if (distance <= primaryAttackRange)
        {
            _currentStrategy = primaryStrategy;
        }
        else
        {
            _currentStrategy = null; 
        }
    }

    protected override void HandleMovement()
    {
        if (playerTransform == null) EnsurePlayerReference();
        if (playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.Translate(direction * (moveSpeed * Time.deltaTime), Space.World);

        if (direction != Vector2.zero)
        {
            transform.up = direction;
        }
    }
}
