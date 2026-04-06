using UnityEngine;

/// <summary>
/// 매우 빠른 속도와 관통력을 가진 레이저 빔을 연사하는 레이저 건입니다.
/// </summary>
public class Gun_Laser : ProjectileWeaponBase<Bullet_Laser>
{
    [Header("Laser Specific Stats")]
    [Tooltip("레이저의 기본 관통 횟수입니다.")]
    [SerializeField] private int basePierce = 3;

    protected override void GenerateBaseFirePattern(FireContext fireContext)
    {
        // 레이저는 기본적으로 정면으로 1발 발사하지만, 
        // ProjectileWeaponBase에서 지원하는 모든 증강 효과를 그대로 받습니다.
        
        int finalDamage = Mathf.RoundToInt(baseDamage * currentWeaponContext.damageMultiplier);
        float finalSpeed = baseBulletSpeed * currentWeaponContext.bulletSpeedMultiplier;
        
        // 레이저는 기본 탄환보다 길쭉한 형태를 가집니다.
        Vector3 laserScale = projectilePrefab != null ? projectilePrefab.transform.localScale : new Vector3(0.5f, 2f, 1f);

        // 레이저 발사 데이터 생성
        BulletData data = new BulletData(Vector2.up, finalDamage, finalSpeed, laserScale, defaultBulletSprite);
        
        // 기본 관통 수치 부여
        data.pierceCount = basePierce;

        fireContext.bullets.Add(data);
    }

    protected override void OnProjectileSpawned(Bullet_Laser projectile, BulletData data)
    {
        // 생성된 레이저 탄환 초기화
        projectile.Init(data);
    }

    public override string GetDebugStats()
    {
        return $"[Gun_Laser] {base.GetDebugStats()} (기본 관통: {basePierce})";
    }
}
