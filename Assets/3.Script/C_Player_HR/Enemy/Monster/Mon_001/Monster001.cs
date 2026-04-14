using UnityEngine;
using Sirenix.OdinInspector;

public class Monster001 : EnemyBase
{
    [Title("몬스터 이동 설정")]
    [LabelText("회전 속도"), SuffixLabel("도/초")]
    [SerializeField] private float rotationSpeed = 10f;

    [Title("공격 전략 할당 (에셋을 드래그하세요)")]
    [LabelText("전략"), Required]
    [SerializeField] private AttackStrategyBase Strategy;

    [InfoBox("@Strategy.description", VisibleIf = "Strategy != null")]
    [Space(10)]

    [LabelText("전환 임계 거리")]
    [SerializeField] private float transitionDistance = 5f;
    
    protected override void Update()
    {
        base.Update();
        if (isDead) return;
        if (Strategy != null)
        {
            Strategy.Execute(this);
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
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}
