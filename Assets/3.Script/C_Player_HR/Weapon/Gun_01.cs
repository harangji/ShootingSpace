using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using ShootingSpace.Core;

/// <summary>
/// 유니티 내장 ObjectPool<T>를 사용하여 최적화된 무기 클래스입니다.
/// </summary>
public class Gun_01 : MonoBehaviour, IWeapon
{
    [Header("Weapon Identity")]
    [SerializeField] private string weaponID = "Gun_01"; // 이 무기의 고유 식별자

    [Header("Augmentations")]
    [SerializeField] private List<AugmentSO> initialAugments = new List<AugmentSO>();
    private List<IAugment> activeModifiers = new List<IAugment>();

    [Header("Base Stats")]
    public float baseFireRate = 2.0f;
    public int baseDamage = 10;
    public float baseBulletSpeed = 20.0f;

    [Header("Settings")]
    public Bullet01 projectile;
    public Transform muzzlePoint;
    public Sprite defaultBulletSprite;

    [Header("Behavior")]
    [SerializeField] private bool autoFire = true;

    // --- 최적화 캐시 데이터 ---
    private IObjectPool<Bullet01> _pool;
    private bool _isPoolInitialized = false;
    private float _lastFireTime;
    private WeaponContext _currentWeaponContext = new WeaponContext();
    private List<BulletData> _cachedBullets = new List<BulletData>();

    public bool AutoFire 
    { 
        get => autoFire; 
        set => autoFire = value; 
    }

    private void Awake()
    {
        InitializePool();
        InitializeAugments(); // 증강 초기화 (검사 로직 포함)
        RefreshWeaponStats();
    }

    private void InitializePool()
    {
        _pool = new ObjectPool<Bullet01>(
            CreateProjectile, OnGetFromPool, OnReleaseToPool, OnDestroyFromPool,
            collectionCheck: true, defaultCapacity: 20, maxSize: 200
        );
        _isPoolInitialized = true;
    }

    private Bullet01 CreateProjectile()
    {
        Bullet01 bullet = Instantiate(projectile);
        bullet.SetPool(_pool);
        return bullet;
    }

    private void OnGetFromPool(Bullet01 bullet) => bullet.gameObject.SetActive(true);
    private void OnReleaseToPool(Bullet01 bullet) => bullet.gameObject.SetActive(false);
    private void OnDestroyFromPool(Bullet01 bullet) => Destroy(bullet.gameObject);

    /// <summary>
    /// 인스펙터에서 설정된 초기 증강들을 검증 후 추가합니다.
    /// </summary>
    private void InitializeAugments()
    {
        foreach (var augmentSO in initialAugments)
        {
            if (augmentSO == null) continue;
            
            // AddAugment를 통해 ID 매칭 및 중복 검사를 동일하게 수행
            AddAugment(augmentSO);
        }
    }

    /// <summary>
    /// 새로운 증강을 추가합니다. ID 매칭 및 중복 여부를 확인합니다.
    /// </summary>
    public bool AddAugment(AugmentSO augment)
    {
        if (augment == null) return false;

        // 1. 타입 및 ID 체크
        if (augment.type == AugmentType.WeaponUnique && augment.targetWeaponID != weaponID)
        {
            Debug.LogWarning($"[Gun] ID 불일치: {augment.augmentName}은 이 무기({weaponID})용이 아님!");
            return false;
        }

        // 2. 중복 체크 (이름 기준)
        if (HasAugment(augment.augmentName))
        {
            Debug.Log($"[Gun] 중복 증강: {augment.augmentName}은 이미 장착되어 있음!");
            return false;
        }

        // 3. 증강 인스턴스 생성 및 추가
        AugmentSO instance = Instantiate(augment);
        activeModifiers.Add(instance);
        
        // 4. 무기 스탯 갱신
        RefreshWeaponStats();
        return true;
    }

    /// <summary>
    /// 지정된 이름을 가진 증강을 제거합니다.
    /// </summary>
    public bool RemoveAugment(string augmentName)
    {
        // 리스트에서 이름이 일치하는 첫 번째 증강SO를 찾습니다냥!
        IAugment target = activeModifiers.Find(m => (m is AugmentSO so) && so.augmentName == augmentName);

        if (target != null)
        {
            activeModifiers.Remove(target);
            
            // 만약 Instantiate된 객체라면 메모리 정리를 위해 파괴 처리한다냥!
            if (target is Object obj) Destroy(obj);

            RefreshWeaponStats(); // 스탯 즉시 갱신!
            Debug.Log($"[Gun] 증강 제거 완료: {augmentName}냥!");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 동일한 이름을 가진 증강이 이미 존재하는지 확인합니다.
    /// </summary>
    private bool HasAugment(string augmentName)
    {
        return activeModifiers.Exists(m => (m is AugmentSO so) && so.augmentName == augmentName);
    }

    /// <summary>
    /// 인터페이스 기반의 외부 증강 추가 (아이템 등)
    /// </summary>
    public void AddModifier(IAugment modifier)
    {
        activeModifiers.Add(modifier);
        RefreshWeaponStats();
    }

    [ContextMenu("Refresh Stats")]
    public void RefreshWeaponStats()
    {
        _currentWeaponContext = new WeaponContext();
        foreach (var mod in activeModifiers)
        {
            mod.ModifyWeapon(_currentWeaponContext);
        }

        FireContext context = new FireContext();
        int finalDamage = Mathf.RoundToInt(baseDamage * _currentWeaponContext.damageMultiplier);
        float finalSpeed = baseBulletSpeed * _currentWeaponContext.bulletSpeedMultiplier;
        
        // --- 프리팹의 원본 스케일을 기초값으로 사용한다냥! ---
        Vector3 baseScale = projectile != null ? projectile.transform.localScale : Vector3.one;
        context.bullets.Add(new BulletData(Vector2.up, finalDamage, finalSpeed, baseScale, defaultBulletSprite));

        foreach (var mod in activeModifiers)
        {
            mod.ModifyFire(context);
        }

        _cachedBullets = context.bullets;
    }

    private void Update()
    {
        float currentFireRate = baseFireRate * _currentWeaponContext.fireRateMultiplier;
        if (autoFire && Time.time >= _lastFireTime + (1f / currentFireRate))
        {
            Fire();
        }
    }

    public void Fire()
    {
        float currentFireRate = baseFireRate * _currentWeaponContext.fireRateMultiplier;
        if (Time.time < _lastFireTime + (1f / currentFireRate)) return;

        foreach (var bulletData in _cachedBullets)
        {
            SpawnBullet(bulletData);
        }

        _lastFireTime = Time.time;
    }

    public void SpawnBullet(BulletData bulletData)
    {
        if (projectile == null || !_isPoolInitialized) return;

        float localAngle = Mathf.Atan2(bulletData.direction.y, bulletData.direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion muzzleRot = muzzlePoint != null ? muzzlePoint.rotation : transform.rotation;
        Quaternion finalRot = muzzleRot * Quaternion.Euler(0, 0, localAngle);
        Vector3 spawnPos = muzzlePoint != null ? muzzlePoint.position : transform.position;

        Bullet01 bullet = _pool.Get();
        bullet.transform.SetPositionAndRotation(spawnPos, finalRot);
        bullet.Init(bulletData);
    }

    public string GetDebugStats()
    {
        float currentFireRate = baseFireRate * _currentWeaponContext.fireRateMultiplier;
        float currentDamage = baseDamage * _currentWeaponContext.damageMultiplier;
        float currentSpeed = baseBulletSpeed * _currentWeaponContext.bulletSpeedMultiplier;

        return $"[Gun Debug] 연사속도: {currentFireRate:F2}/s (x{_currentWeaponContext.fireRateMultiplier:F2}), " +
               $"데미지: {currentDamage:F1} (x{_currentWeaponContext.damageMultiplier:F2}), " +
               $"탄속: {currentSpeed:F1} (x{_currentWeaponContext.bulletSpeedMultiplier:F2}), " +
               $"탄환수: {_cachedBullets.Count}개";
    }
}
