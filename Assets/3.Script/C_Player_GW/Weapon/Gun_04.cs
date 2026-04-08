using UnityEngine;

/// <summary>
/// ProjectileWeaponBase를 상속받아 구현한 유도탄 발사 무기 클래스입니다.
/// Bullet_04를 발사합니다.
/// </summary>
public class Gun_04 : ProjectileWeaponBase<Bullet_04>
{
    protected override void GenerateBaseFirePattern(FireContext fireContext)
    {
        // Gun_04의 기본 패턴: 정면(Vector2.up)으로 1발 발사
        // Bullet_04는 자체적으로 가장 가까운 적을 찾아가므로, 초기 방향은 중요하지 않지만
        // 일관성을 위해 Vector2.up으로 설정합니다.
        int finalDamage = Mathf.RoundToInt(baseDamage * currentWeaponContext.damageMultiplier);
        float finalSpeed = baseBulletSpeed * currentWeaponContext.bulletSpeedMultiplier;
        Vector3 baseScale = projectilePrefab != null ? projectilePrefab.transform.localScale : Vector3.one;

        fireContext.bullets.Add(new BulletData(Vector2.up, finalDamage, finalSpeed, baseScale, defaultBulletSprite));
    }

    protected override void OnProjectileSpawned(Bullet_04 projectile, BulletData data)
    {
        // 생성된 탄환(Bullet_04) 초기화
        projectile.Init(data);
    }

    public override string GetDebugStats()
    {
        return $"[Gun_04] {base.GetDebugStats()}";
    }
}