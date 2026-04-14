using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "DashAttack", menuName = "ShootingSpace/Enemy/Strategies/Dash")]
public class DashAttack : AttackStrategyBase
{
    [Title("돌진 세부 설정")]
    [SerializeField] private float warningTime = 0.5f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashWidth = 1.5f;
    [SerializeField] private float dashDistance = 10f;
    [SerializeField] private float dashCooldown = 3f;

    private float _lastDashTime;
    private bool _isDashing = false;

    public override void Execute(EnemyBase enemy)
    {
        if (_isDashing || Time.time < _lastDashTime + dashCooldown) return;
        enemy.StartCoroutine(DashRoutine(enemy));
    }

    private IEnumerator DashRoutine(EnemyBase enemy)
    {
        _isDashing = true;
        Transform player = enemy.GetPlayerTransform();
        if (player == null) yield break;

        SpriteRenderer sr = enemy.GetSpriteRenderer();
        EnemyIndicator indicator = enemy.GetIndicator();

        // 1. 경고 단계 (인디케이터 ON)
        float originalSpeed = enemy.GetMoveSpeed();
        enemy.SetMoveSpeed(0);
        
        Vector2 direction = ((Vector2)player.position - (Vector2)enemy.transform.position).normalized;
        enemy.transform.up = direction;

        if (indicator != null)
        {
            Vector3 indicatorPos = enemy.transform.position + enemy.transform.up * (dashDistance * 0.5f);
            indicator.Show(indicatorPos, enemy.transform.rotation, new Vector2(dashWidth, dashDistance));
        }

        float elapsed = 0f;
        while (elapsed < warningTime)
        {
            elapsed += Time.deltaTime;
            if (indicator != null) indicator.SetProgress(elapsed / warningTime);
            yield return null;
        }

        // 2. 공격 단계 (인디케이터 OFF)
        if (indicator != null) indicator.Hide();
        enemy.SetMoveSpeed(originalSpeed * 4f);
        yield return new WaitForSeconds(dashDuration);

        // 3. 복구 단계
        enemy.SetMoveSpeed(originalSpeed);
        _lastDashTime = Time.time;
        _isDashing = false;
    }
}
