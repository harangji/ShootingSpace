using System.Collections.Generic;
using UnityEngine;
using ShootingSpace.Core;

/// <summary>
/// 기본적인 무기 구현 클래스입니다.
/// 인스펙터에서 할당 가능한 AugmentSO 리스트를 관리합니다.
/// </summary>
public class Gun_01 : MonoBehaviour, IWeapon
{
    [Header("Augmentations")]
    // 인스펙터에서 직접 드래그 앤 드롭으로 할당 가능한 리스트
    [SerializeField] private List<AugmentSO> initialAugments = new List<AugmentSO>();
    
    // 런타임에 실제로 사용할 독립적인 증강 인스턴스 리스트
    private List<IAugment> activeModifiers = new List<IAugment>();

    [Header("Settings")]
    public GameObject projectile;
    public Transform muzzlePoint;
    public float fireRate = 2.0f;

    [Header("Behavior")]
    [SerializeField] private bool autoFire = true;

    private float _lastFireTime;

    public bool AutoFire 
    { 
        get => autoFire; 
        set => autoFire = value; 
    }

    private void Awake()
    {
        // 에셋 원본 대신 복제본을 사용하여 각 무기마다 개별적인 데이터 수정을 가능케 함
        InitializeAugments();
    }

    private void InitializeAugments()
    {
        foreach (var augmentSO in initialAugments)
        {
            if (augmentSO == null) continue;
            
            // ScriptableObject를 복제하여 런타임용 인스턴스 생성
            AugmentSO instance = Instantiate(augmentSO);
            activeModifiers.Add(instance);
        }
    }

    private void Update()
    {
        if (autoFire && Time.time >= _lastFireTime + (1f / fireRate))
        {
            Fire();
        }
    }

    public void Fire()
    {
        if (Time.time < _lastFireTime + (1f / fireRate)) return;

        FireContext context = new FireContext();
        Vector3 fireDir = muzzlePoint != null ? muzzlePoint.up : transform.up;

        // 발사 데이터 구성 (Y축 정면 기준)
        context.bullets.Add(new BulletData
        {
            direction = (Vector2)fireDir,
            damage = 10,
            speed = 20
        });

        // 런타임에 할당된 모든 증강 효과 적용
        foreach (var mod in activeModifiers)
        {
            mod.ModifyFire(context);
        }

        // 증강이 적용된 모든 총알 데이터 소환
        foreach (var bulletData in context.bullets)
        {
            SpawnBullet(bulletData);
        }

        _lastFireTime = Time.time;
    }

    public void SpawnBullet(BulletData bulletData)
    {
        if (projectile == null)
        {
            Debug.LogError("Projectile 프리팹이 설정되지 않았습니다.");
            return;
        }

        // 총알 데이터의 '방향(direction)'을 기반으로 회전값 설정
        float angle = Mathf.Atan2(bulletData.direction.y, bulletData.direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion spawnRot = Quaternion.Euler(0, 0, angle);
        
        Vector3 spawnPos = muzzlePoint != null ? muzzlePoint.position : transform.position;

        GameObject bulletObj = Instantiate(projectile, spawnPos, spawnRot);

        if (bulletObj.TryGetComponent<Bullet01>(out var bullet))
        {
            bullet.Init(bulletData);
        }
    }
}
