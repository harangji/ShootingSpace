using UnityEngine;

/// <summary>
/// ProjectileWeaponBase를 상속받아 구현한 유도탄 권총 클래스입니다.
/// 발사되는 총알은 가장 가까운 적을 추적하는 Bullet_03 타입입니다.
/// </summary>
public class Gun_03 : ProjectileWeaponBase<Bullet_03>
{
    protected override void GenerateBaseFirePattern(FireContext fireContext)
    {
        // Gun_03의 기본 패턴: 정면(Vector2.up)으로 1발 발사
        int finalDamage = Mathf.RoundToInt(baseDamage * currentWeaponContext.damageMultiplier);
        float finalSpeed = baseBulletSpeed * currentWeaponContext.bulletSpeedMultiplier;
        Vector3 baseScale = projectilePrefab != null ? projectilePrefab.transform.localScale : Vector3.one;

        fireContext.bullets.Add(new BulletData(Vector2.up, finalDamage, finalSpeed, baseScale, defaultBulletSprite));
    }

    protected override void OnProjectileSpawned(Bullet_03 projectile, BulletData data)
    {
        // 생성된 탄환(Bullet_03) 초기화
        projectile.Init(data);
    }

    public override string GetDebugStats()
    {
        return $"[Gun_03] {base.GetDebugStats()}";
    }
}
