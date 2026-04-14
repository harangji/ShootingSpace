using UnityEngine;
using System.Collections;
using ShootingSpace.Core;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ExplodeSplitAttack", menuName = "ShootingSpace/Enemy/Strategies/ExplodeSplit")]
public class ExplodeSplitAttack : AttackStrategyBase
{
    [Title("폭발 세부 설정")]
    [SerializeField] private float chargeTime = 2.0f;
    [SerializeField] private int projectileCount = 8;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 20;

    private bool _isAttacking = false;

    public override void Execute(EnemyBase enemy)
    {
        if (_isAttacking) return;
        enemy.StartCoroutine(ExplodeRoutine(enemy));
    }

    private IEnumerator ExplodeRoutine(EnemyBase enemy)
    {
        _isAttacking = true;
        EnemyIndicator indicator = enemy.GetIndicator();

        // 1. 충전 단계 (인디케이터 ON)
        if (indicator != null)
        {
            indicator.Show(enemy.transform.position, Quaternion.identity, Vector2.one * explosionRadius * 2f);
        }

        float elapsed = 0f;
        while (elapsed < chargeTime)
        {
            elapsed += Time.deltaTime;
            if (indicator != null)
            {
                indicator.SetProgress(elapsed / chargeTime);
                indicator.transform.position = enemy.transform.position;
            }
            enemy.GetSpriteRenderer().transform.localPosition = Random.insideUnitCircle * 0.1f;
            yield return null;
        }

        // 2. 폭발 단계 (인디케이터 OFF)
        if (indicator != null) indicator.Hide();
        enemy.GetSpriteRenderer().transform.localPosition = Vector3.zero;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemy.transform.position, explosionRadius);
        foreach (var col in hitColliders)
        {
            if (col.CompareTag("Player") && col.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(explosionDamage);
            }
        }

        float angleStep = 360f / projectileCount;
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = i * angleStep;
            Debug.Log($"[ExplodeSplit] 투사체 발사! 각도: {angle}");
        }

        enemy.TakeDamage(9999);
    }
}
