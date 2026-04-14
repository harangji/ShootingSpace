using UnityEngine;
using System.Collections;
using ShootingSpace.Core;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "LaserAttack", menuName = "ShootingSpace/Enemy/Strategies/Laser")]
public class LaserAttack : AttackStrategyBase
{
    [Title("레이저 세부 설정")]
    [SerializeField] private float chargeTime = 1.5f;
    [SerializeField] private float fireDuration = 1.0f;
    [SerializeField] private float cooldown = 4f;
    [SerializeField] private float laserWidth = 1.0f;
    [SerializeField] private float laserDistance = 20f;
    [SerializeField] private int damage = 5;

    private float _lastAttackTime;
    private bool _isAttacking = false;

    public override void Execute(EnemyBase enemy)
    {
        if (_isAttacking || Time.time < _lastAttackTime + cooldown) return;
        enemy.StartCoroutine(LaserRoutine(enemy));
    }

    private IEnumerator LaserRoutine(EnemyBase enemy)
    {
        _isAttacking = true;
        Transform player = enemy.GetPlayerTransform();
        if (player == null) yield break;

        EnemyIndicator indicator = enemy.GetIndicator();

        // 1. 충전 단계 (인디케이터 ON)
        if (indicator != null)
        {
            Vector3 indicatorPos = enemy.transform.position + enemy.transform.up * (laserDistance * 0.5f);
            indicator.Show(indicatorPos, enemy.transform.rotation, new Vector2(laserWidth, laserDistance));
        }

        float elapsed = 0f;
        while (elapsed < chargeTime)
        {
            elapsed += Time.deltaTime;
            if (indicator != null) indicator.SetProgress(elapsed / chargeTime);
            
            if (player != null)
            {
                Vector2 direction = ((Vector2)player.position - (Vector2)enemy.transform.position).normalized;
                enemy.transform.up = direction;
            }
            yield return null;
        }

        // 2. 발사 단계 (인디케이터 OFF)
        if (indicator != null) indicator.Hide();

        float fireElapsed = 0f;
        while (fireElapsed < fireDuration)
        {
            fireElapsed += Time.deltaTime;
            RaycastHit2D hit = Physics2D.BoxCast(enemy.transform.position, new Vector2(laserWidth, 0.1f), 
                enemy.transform.eulerAngles.z, enemy.transform.up, laserDistance, LayerMask.GetMask("Player"));

            if (hit.collider != null && hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
            }
            yield return null;
        }

        _lastAttackTime = Time.time;
        _isAttacking = false;
    }
}
