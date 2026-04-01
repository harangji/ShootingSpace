using UnityEngine;

/// <summary>
/// 부채꼴 모양으로 3발을 발사하는 트리플 건 클래스입니다.
/// </summary>
public class Gun_02 : ProjectileWeaponBase<Bullet01>
{
    [Header("Gun_02 Settings")]
    [SerializeField] private float spreadAngle = 15f;

    protected override void GenerateBaseFirePattern(FireContext fireContext)
    {
        int finalDamage = Mathf.RoundToInt(baseDamage * currentWeaponContext.damageMultiplier);
        float finalSpeed = baseBulletSpeed * currentWeaponContext.bulletSpeedMultiplier;
        Vector3 baseScale = projectilePrefab != null ? projectilePrefab.transform.localScale : Vector3.one;

        // 3발 발사 (중앙, 왼쪽, 오른쪽)
        float[] angles = { 0, -spreadAngle, spreadAngle };

        foreach (float angle in angles)
        {
            // 각도에 따른 방향 벡터 계산 (Vector2.up 기준)
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
            fireContext.bullets.Add(new BulletData(direction, finalDamage, finalSpeed, baseScale, defaultBulletSprite));
        }
    }

    protected override void OnProjectileSpawned(Bullet01 projectile, BulletData data)
    {
        projectile.Init(data);
    }

    public override string GetDebugStats()
    {
        return $"[Gun_02] {base.GetDebugStats()} (퍼짐: {spreadAngle}도)";
    }
}
