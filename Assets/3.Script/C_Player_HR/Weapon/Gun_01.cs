using UnityEngine;

/// <summary>
/// ProjectileWeaponBase를 상속받아 구현한 기본 권총 클래스입니다.
/// </summary>
public class Gun_01 : ProjectileWeaponBase<Bullet_01>
{
    protected override void GenerateBaseFirePattern(FireContext fireContext)
    {
        // Gun_01의 기본 패턴: 정면(Vector2.up)으로 1발 발사
        int finalDamage = Mathf.RoundToInt(baseDamage * currentWeaponContext.damageMultiplier);
        float finalSpeed = baseBulletSpeed * currentWeaponContext.bulletSpeedMultiplier;
        Vector3 baseScale = projectilePrefab != null ? projectilePrefab.transform.localScale : Vector3.one;

        fireContext.bullets.Add(new BulletData(Vector2.up, finalDamage, finalSpeed, baseScale, defaultBulletSprite));
    }

    protected override void OnProjectileSpawned(Bullet_01 projectile, BulletData data)
    {
        // 생성된 탄환(Bullet01) 초기화
        projectile.Init(data);
    }

    public override string GetDebugStats()
    {
        return $"[Gun_01] {base.GetDebugStats()}";
    }
}
