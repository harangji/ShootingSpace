using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 발사체(Projectile) 기반 무기의 공통 로직을 처리하는 제네릭 베이스 클래스입니다.
/// </summary>
/// <typeparam name="T">사용할 탄환의 컴포넌트 타입</typeparam>
public abstract class ProjectileWeaponBase<T> : WeaponBase where T : Component, IPoolable<T>
{
    [Header("Base Stats")]
    [SerializeField] protected float baseFireRate = 2.0f;
    [SerializeField] protected int baseDamage = 10;
    [SerializeField] protected float baseBulletSpeed = 20.0f;
    [SerializeField] protected bool autoFire = true;

    [Header("Resources")]
    [SerializeField] protected T projectilePrefab;
    [SerializeField] protected Transform muzzlePoint;
    [SerializeField] protected Sprite defaultBulletSprite;
    [SerializeField] protected Transform bulletParent;

    // 실시간 계산용 데이터
    protected GenericObjectPool<T> pool;
    protected WeaponContext currentWeaponContext = new WeaponContext();
    protected List<BulletData> cachedBullets = new List<BulletData>();
    protected float lastFireTime;

    protected override void Awake()
    {
        InitializeBulletParent();
        
        // 공용 제네릭 풀 초기화
        if (projectilePrefab != null)
        {
            pool = new GenericObjectPool<T>(projectilePrefab, bulletParent);
        }

        base.Awake();
        
        // 초기 스탯 갱신
        RefreshWeaponStats(new PlayerContext(), new List<ItemAugmentSO>());
    }

    protected virtual void Update()
    {
        if (!autoFire) return;

        float currentFireRate = baseFireRate * currentWeaponContext.fireRateMultiplier;
        if (Time.time >= lastFireTime + (1f / currentFireRate))
        {
            Fire();
        }
    }

    /// <summary>
    /// 무기 발사를 실행합니다. (WeaponBase 구현)
    /// </summary>
    public override void Fire()
    {
        float currentFireRate = baseFireRate * currentWeaponContext.fireRateMultiplier;
        if (Time.time < lastFireTime + (1f / currentFireRate)) return;

        // 캐싱된 모든 탄환 데이터를 순회하며 발사
        foreach (var bulletData in cachedBullets)
        {
            SpawnProjectile(bulletData);
        }

        lastFireTime = Time.time;
    }

    /// <summary>
    /// 개별 탄환을 생성하고 초기화합니다.
    /// </summary>
    protected virtual void SpawnProjectile(BulletData bulletData)
    {
        if (pool == null) return;

        // 1. 풀에서 탄환 가져오기
        T projectile = pool.Get();

        // 2. 위치 및 회전 설정 (총구 방향 기준)
        float localAngle = Mathf.Atan2(bulletData.direction.y, bulletData.direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion muzzleRot = muzzlePoint != null ? muzzlePoint.rotation : transform.rotation;
        Quaternion finalRot = muzzleRot * Quaternion.Euler(0, 0, localAngle);
        Vector3 spawnPos = muzzlePoint != null ? muzzlePoint.position : transform.position;

        projectile.transform.SetPositionAndRotation(spawnPos, finalRot);

        // 3. 탄환 초기 데이터 설정 (Bullet01 같은 경우 Init 호출 필요)
        // 자식 클래스에서 특수한 초기화가 필요하다면 이 메서드를 오버라이드하거나,
        // 인터페이스를 통해 공통 초기화 로직을 호출할 수 있습니다.
        OnProjectileSpawned(projectile, bulletData);
    }

    /// <summary>
    /// 탄환이 생성된 후 구체적인 데이터를 설정합니다. 자식 클래스에서 구현이 필요할 수 있습니다.
    /// </summary>
    protected abstract void OnProjectileSpawned(T projectile, BulletData data);

    /// <summary>
    /// 무기의 최종 발사 스탯을 갱신합니다. (WeaponBase 구현)
    /// </summary>
    public override void RefreshWeaponStats(PlayerContext playerContext, List<ItemAugmentSO> globalItems)
    {
        currentWeaponContext = new WeaponContext();

        // 1. 전역 아이템 및 플레이어 데미지 배율 적용
        foreach (var item in globalItems) if (item != null) item.ModifyWeapon(currentWeaponContext);
        
        // 필살기 배율과 플레이어 공통 데미지 배율 합산 적용
        currentWeaponContext.damageMultiplier *= (playerContext.damageMultiplier * playerContext.ultimateDamageMultiplier);
        currentWeaponContext.fireRateMultiplier *= playerContext.ultimateFireRateMultiplier;

        // 2. 무기 고유 증강 적용 (WeaponBase의 activeModifiers 사용)
        foreach (var mod in activeModifiers) mod.ModifyWeapon(currentWeaponContext);

        // 3. 발사 데이터 기초 생성 (FireContext)
        FireContext fireContext = new FireContext();
        
        // 자식 클래스에서 "기본적으로 몇 발을 어느 방향으로 쏠지" 정의합니다.
        GenerateBaseFirePattern(fireContext);

        // 4. 발사 데이터 증강 수정 (증강들에 의해 총알 수가 늘어나거나 속성이 변함)
        foreach (var item in globalItems) if (item != null) item.ModifyFire(fireContext);
        foreach (var mod in activeModifiers) mod.ModifyFire(fireContext);

        // 5. 최종 결과 캐싱
        cachedBullets = fireContext.bullets;
    }

    /// <summary>
    /// 무기 고유의 기본 발사 패턴(몇 발을 어디로 쏠지)을 생성합니다.
    /// </summary>
    protected abstract void GenerateBaseFirePattern(FireContext fireContext);

    /// <summary>
    /// 탄환 컨테이너를 초기화합니다.
    /// </summary>
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
