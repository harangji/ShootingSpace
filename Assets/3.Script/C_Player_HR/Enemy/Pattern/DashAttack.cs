using System;
using UnityEngine;
using System.Threading.Tasks;
using ShootingSpace.Core;
using Sirenix.OdinInspector;

[Title("돌진 공격")]
public class DashAttack : AttackStrategyBase
{
    [Title("돌진 세부 설정")] [LabelText("경고 시간"), SuffixLabel("초"), Tooltip("돌진 전 경고 표시가 유지되는 시간")] [SerializeField]
    private float warningTime = 0.5f;

    [LabelText("돌진 지속 시간"), SuffixLabel("초"), Tooltip("실제 돌진이 수행되는 시간")] [SerializeField]
    private float dashDuration = 0.3f;

    [LabelText("돌진 범위 폭"), Tooltip("돌진 공격의 너비")] [SerializeField]
    private float dashWidth = 1.5f;

    [LabelText("돌진 거리"), Tooltip("돌진 공격의 최대 사거리")] [SerializeField]
    private float dashDistance = 10f;

    [LabelText("돌진 쿨타임"), SuffixLabel("초"), Tooltip("돌진 공격 사용 후 재사용 대기 시간")] [SerializeField]
    private float dashCooldown = 3f;

    [LabelText("돌진 속도"), Tooltip("돌진 시 이동 속도")] [SerializeField]
    private float dashSpeed = 20f;

    [LabelText("돌진 데미지"), Tooltip("돌진 공격 시 가하는 데미지")] [SerializeField]
    private int dashDamage = 20;

    private float _lastDashTime;
    private bool _isDashing = false;
    
    private void OnValidate()
    {
        damage = dashDamage;
        description = $"전방으로 {dashDistance}m 거리를 {dashSpeed} 속도로 돌진하여 {dashDamage}의 데미지를 입힙니다. " +
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
        Vector2 targetPos = GameManager.Instance.playerTransform.position;
        Vector2 direction = (targetPos - (Vector2)enemy.transform.position).normalized;
        enemy.transform.up = direction;

        Vector2 indicatorPos = (Vector2)enemy.transform.position + (Vector2)enemy.transform.up * (dashDistance * 0.5f);
        indicator.Show(indicatorPos, enemy.transform.rotation, new Vector2(dashWidth, dashDistance));

        float startTime = Time.time;
        while (Time.time < startTime + warningTime)
        {
            indicator.SetProgress((Time.time - startTime) / warningTime);
            await Task.Yield();
        }
        
        enemy.IsPerformingSpecialAttack = true;
        var hitTargets = new System.Collections.Generic.HashSet<IDamageable>();
        
        // 2. 공격 단계
        indicator.Hide();

        float dashTimer = 0f;
        while (dashTimer < dashDuration)
        {
            Vector2 currentPos = enemy.transform.position;
            currentPos += direction * (dashSpeed * Time.deltaTime);
            enemy.transform.position = currentPos;

            // 여기서 플레이어와 충돌 체크 후 데미지 적용 - 안되고있음 레이어검사 수정 TODO
            Collider2D hit = Physics2D.OverlapBox(enemy.eCollider.bounds.center, new Vector2(dashWidth, 0.5f),
                enemy.transform.eulerAngles.z, LayerMask.GetMask("Player"));
            if (hit != null && hit.CompareTag("Player") && hit.TryGetComponent<IDamageable>(out var damageable))
            {
                if (hitTargets.Add(damageable))
                {
                    damageable.TakeDamage(dashDamage);
                }
            }

            dashTimer += Time.deltaTime;
            await Task.Yield();
        }

        // 3. 복구 단계
        enemy.IsPerformingSpecialAttack = false;
        _lastDashTime = Time.time;
        _isDashing = false;
    }
}