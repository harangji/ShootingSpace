using UnityEngine;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using DG.Tweening;

[Title("돌진 공격")]
public class DashAttack : AttackStrategyBase
{
    [Title("돌진 세부 설정")] 
    [LabelText("경고 시간"), SuffixLabel("초"), SerializeField]
    private float warningTime = 0.5f;

    [LabelText("돌진 총 시간"), SuffixLabel("초"), SerializeField] 
    private float dashDuration = 0.3f;

    [LabelText("돌진 범위 폭"), SerializeField]
    private float dashWidth = 1.5f;

    [LabelText("돌진 거리"), Tooltip("돌진 공격의 최대 사거리"),SerializeField]
    private float dashDistance = 10f;

    [LabelText("돌진 쿨타임"), SuffixLabel("초"), SerializeField]
    private float dashCooldown = 3f;

    [LabelText("돌진 데미지"), SerializeField]
    private int dashDamage = 20;

    private float _lastDashTime;
    private bool _isDashing = false;
    
    private void OnValidate()
    {
        damage = dashDamage;
        description = $"전방으로 {dashDistance}m 거리를 돌진하여 {dashDamage}의 데미지를 입힙니다. " +
                      $"{warningTime}초간 경고 후 {dashDuration}초 동안 이동하며, " +
                      $"{dashCooldown}초의 재사용 대기 시간을 가집니다.";
    }

    public override bool CanExecute()
    {
        return !_isDashing && Time.time >= _lastDashTime + dashCooldown;
    }

    public override async Task ExecuteAsync(EnemyBase enemy)
    {
        if (!CanExecute()) return;

        _isDashing = true;

        EnemyIndicator indicator = enemy.GetIndicator();

        // 1. 경고 단계
        Vector2 direction = (GameManager.Instance.playerTransform.position - enemy.transform.position).normalized;
        enemy.transform.up = direction;

        Vector2 indicatorPos = (Vector2)enemy.transform.position + (Vector2)enemy.transform.up * (dashDistance * 0.5f);
        indicator.Show(indicatorPos, enemy.transform.rotation, new Vector2(dashWidth, dashDistance));

        float startTime = Time.time;
        while (Time.time < startTime + warningTime)
        {
            indicator.SetProgress((Time.time - startTime) / warningTime);
            await Task.Yield();
        }
        
        // 2. 공격 단계
        indicator.Hide();
        enemy.IsPerformingSpecialAttack = true;
        var hitTargets = new System.Collections.Generic.HashSet<IDamageable>();
        
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = startPos + (Vector3)direction * dashDistance;

        // DOTween으로 돌진 수행 (Ease.InQuad로 처음에 느렸다가 빨라짐)
        await enemy.transform.DOMove(endPos, dashDuration)
            .SetEase(Ease.InQuad)
            .OnUpdate(() =>
            {
                Collider2D hit = Physics2D.OverlapBox(enemy.transform.position + enemy.transform.up * (dashDistance * 0.5f), 
                    new Vector2(dashWidth, dashDistance), enemy.transform.eulerAngles.z, LayerMask.GetMask("Player"));
                
                if (hit != null && hit.CompareTag("Player") && hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (hitTargets.Add(damageable))
                    {
                        damageable.TakeDamage(dashDamage);
                    }
                }
            })
            .AsyncWaitForCompletion();

        // 3. 복구 단계
        enemy.IsPerformingSpecialAttack = false;
        _lastDashTime = Time.time;
        _isDashing = false;
    }
}