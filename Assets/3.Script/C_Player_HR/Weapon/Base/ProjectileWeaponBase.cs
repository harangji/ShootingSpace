using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 모든 발사체(Projectile) 기반 무기의 공통 로직을 처리하는 제네릭 베이스 클래스입니다.
/// </summary>
public abstract class ProjectileWeaponBase<T> : WeaponBase where T : Component, IPoolable<T>
{
    [TitleGroup("발사 기본 스탯")]
    [LabelText("기본 연사 속도"), SuffixLabel("발/초")]
    [SerializeField] protected float baseFireRate = 2.0f;

    [LabelText("기본 데미지")]
    [SerializeField] protected int baseDamage = 10;

    [LabelText("기본 탄속")]
    [SerializeField] protected float baseBulletSpeed = 20.0f;

    [LabelText("자동 발사 여부")]
    [SerializeField] protected bool autoFire = true;

    [TitleGroup("필요 리소스")]
    [LabelText("탄환 프리팹"), Required]
    [SerializeField] protected T projectilePrefab;

    [LabelText("총구(Muzzle) 위치")]
    [SerializeField] protected Transform muzzlePoint;

    [LabelText("기본 탄환 스프라이트")]
    [SerializeField] protected Sprite defaultBulletSprite;

    [LabelText("탄환 생성 부모")]
    [SerializeField] protected Transform bulletParent;

    [LabelText("풀 미리 생성 개수")]
    [SerializeField] protected int preWarmCount = 20;

    protected GenericObjectPool<T> pool;
    protected WeaponContext currentWeaponContext = new WeaponContext();
    protected List<BulletData> cachedBullets = new List<BulletData>();
    protected float lastFireTime;

    protected override void Awake()
    {
        InitializeBulletParent();
        if (projectilePrefab != null)
            pool = new GenericObjectPool<T>(projectilePrefab, bulletParent, preWarmCount, 200, preWarmCount);

        base.Awake();
        RefreshWeaponStats(new PlayerContext(), new List<ItemAugmentSO>());
    }

    protected virtual void Update()
    {
        if (!autoFire) return;

        float currentFireRate = baseFireRate * currentWeaponContext.fireRateMultiplier;
        if (Time.time >= lastFireTime + (1f / currentFireRate)) Fire();
    }

    public override void Fire()
    {
        float currentFireRate = baseFireRate * currentWeaponContext.fireRateMultiplier;
        if (Time.time < lastFireTime + (1f / currentFireRate)) return;

        int burstCount = (cachedBullets.Count > 0 && cachedBullets[0].multiShotCount > 0) ? (cachedBullets[0].multiShotCount + 1) : 1;

        StartCoroutine(FireCoroutine(burstCount, 0.2f));

        lastFireTime = Time.time;
    }

    private IEnumerator FireCoroutine(int burstCount, float interval)
    {
        for (int i = 0; i < burstCount; i++)
        {
            foreach (var bulletData in cachedBullets) SpawnProjectile(bulletData);
            yield return new WaitForSeconds(interval);
        }
    }

    protected virtual void SpawnProjectile(BulletData bulletData)
    {
        if (pool == null) return;

        T projectile = pool.Get();
        float localAngle = Mathf.Atan2(bulletData.direction.y, bulletData.direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion muzzleRot = muzzlePoint != null ? muzzlePoint.rotation : transform.rotation;
        Quaternion finalRot = muzzleRot * Quaternion.Euler(0, 0, localAngle);
        Vector3 spawnPos = muzzlePoint != null ? muzzlePoint.position : transform.position;

        projectile.transform.SetPositionAndRotation(spawnPos, finalRot);
        OnProjectileSpawned(projectile, bulletData);
    }

    protected abstract void OnProjectileSpawned(T projectile, BulletData data);

    public override void RefreshWeaponStats(PlayerContext playerContext, List<ItemAugmentSO> globalItems)
    {
        currentWeaponContext = new WeaponContext();

        foreach (var item in globalItems) if (item != null) item.ModifyWeapon(currentWeaponContext);
        
        currentWeaponContext.damageMultiplier *= (playerContext.damageMultiplier * playerContext.ultimateDamageMultiplier);
        currentWeaponContext.fireRateMultiplier *= playerContext.ultimateFireRateMultiplier;

        foreach (var mod in activeModifiers) mod.ModifyWeapon(currentWeaponContext);

        FireContext fireContext = new FireContext();
        GenerateBaseFirePattern(fireContext);

        foreach (var item in globalItems) if (item != null) item.ModifyFire(fireContext);
        foreach (var mod in activeModifiers) mod.ModifyFire(fireContext);

        cachedBullets = fireContext.bullets;
    }

    protected abstract void GenerateBaseFirePattern(FireContext fireContext);

    protected void InitializeBulletParent()
    {
        if (bulletParent == null || bulletParent.IsChildOf(transform))
        {
            GameObject container = GameObject.Find("Bullets_Container");
            if (container == null) container = new GameObject("Bullets_Container");
            bulletParent = container.transform;
        }
    }

    public override string GetDebugStats()
    {
        float currentFireRate = baseFireRate * currentWeaponContext.fireRateMultiplier;
        float currentDamage = baseDamage * currentWeaponContext.damageMultiplier;
        float currentSpeed = baseBulletSpeed * currentWeaponContext.bulletSpeedMultiplier;
        return $" 연사: {currentFireRate:F2}/s, 데미지: {currentDamage:F1}, 탄속: {currentSpeed:F1}, 탄수: {cachedBullets.Count}";
    }
}
