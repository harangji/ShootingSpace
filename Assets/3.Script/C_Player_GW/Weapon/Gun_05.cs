using UnityEngine;

/// <summary>
/// ProjectileWeaponBase를 상속받아 구현한 공전 행성 클래스입니다.
/// 발사되는 총알은 플레이어 주변을 회전하는 Bullet_05 타입입니다.
/// </summary>
public class Gun_05 : ProjectileWeaponBase<Bullet_05>
{
    protected override void Awake()
    {
        base.Awake(); // ProjectileWeaponBase의 Awake 호출
        baseFireRate = 0.2f; // 5초에 1번 발사 (1/5)
    }

    protected override void Update()
    {
        if (!autoFire) return;

        if (Time.time >= lastFireTime + (1f / baseFireRate)) Fire();
    }

    protected override void GenerateBaseFirePattern(FireContext fireContext)
    {
        // Gun_05의 기본 패턴: 정면(Vector2.up)으로 1발 발사
        int finalDamage = Mathf.RoundToInt(baseDamage * currentWeaponContext.damageMultiplier);
        float finalSpeed = 2f;
        Vector3 baseScale = projectilePrefab != null ? projectilePrefab.transform.localScale : Vector3.one;

        BulletData bullet = new BulletData(Vector2.up, finalDamage, finalSpeed, baseScale, defaultBulletSprite);
        bullet.multiShotCount = 2; // 3회 연속 발사를 위해 multiShotCount를 2로 설정 (burstCount = multiShotCount + 1)
        bullet.multiShotInterval = bullet.spinSpeed / (bullet.multiShotCount + 1);
        fireContext.bullets.Add(bullet);
    }

    protected override void OnProjectileSpawned(Bullet_05 projectile, BulletData data)
    {
        // 생성된 탄환(Bullet_05) 초기화
        projectile.Init(data);
    }

    public override string GetDebugStats()
    {
        return $"[Gun_05] {base.GetDebugStats()}";
    }
}
