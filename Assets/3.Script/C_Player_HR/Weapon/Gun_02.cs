using UnityEngine;

/// <summary>
/// 랜덤한 방향으로 1발의 폭발 탄환을 발사하는 랜덤 건 클래스입니다.
/// </summary>
public class Gun_02 : ProjectileWeaponBase<Bullet_02>
{
    protected override void GenerateBaseFirePattern(FireContext fireContext)
    {
        int finalDamage = Mathf.RoundToInt(baseDamage * currentWeaponContext.damageMultiplier);
        float finalSpeed = baseBulletSpeed * currentWeaponContext.bulletSpeedMultiplier;
        Vector3 baseScale = projectilePrefab != null ? projectilePrefab.transform.localScale : Vector3.one;

        // 기초 데이터만 생성 (방향은 발사 시점에 결정됨)
        fireContext.bullets.Add(new BulletData(Vector2.up, finalDamage, finalSpeed, baseScale, defaultBulletSprite));
    }

    /// <summary>
    /// 발사 직전에 탄환의 방향을 랜덤으로 다시 정합니다.
    /// </summary>
    public override void Fire()
    {
        // 발사할 때마다 모든 탄환 데이터의 방향을 랜덤으로 갱신!
        for (int i = 0; i < cachedBullets.Count; i++)
        {
            BulletData data = cachedBullets[i];
            data.direction = Random.insideUnitCircle.normalized;
            cachedBullets[i] = data; // 구조체이므로 다시 대입
        }

        base.Fire();
    }

    protected override void OnProjectileSpawned(Bullet_02 projectile, BulletData data)
    {
        projectile.Init(data);
    }

    public override string GetDebugStats()
    {
        return $"[Gun_02] {base.GetDebugStats()} (매 발사 랜덤 모드)";
    }
}
