using UnityEngine;
using Sirenix.OdinInspector;

public class Monster002 : EnemyBase
{
    [Title("몬스터 이동 설정")]
    [LabelText("추가 속도 배율"), Range(1.0f, 3.0f)]
    [SerializeField] private float speedMultiplier = 1.5f;

    [Title("공격 전략 할당")]
    [LabelText("돌격 공격 전략"), Required]
    [SerializeField] private AttackStrategyBase attackStrategy;

    [InfoBox("@attackStrategy.description", VisibleIf = "attackStrategy != null")]
    [Space(5)]

    [LabelText("공격 사거리")]
    [SerializeField] private float attackRange = 15f;

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

        if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            if (attackStrategy != null)
            {
                attackStrategy.Execute(this);
            }
        }
    }

    protected override void HandleMovement()
    {
        transform.Translate(Vector3.up * (moveSpeed * speedMultiplier * Time.deltaTime), Space.Self);
    }
}
