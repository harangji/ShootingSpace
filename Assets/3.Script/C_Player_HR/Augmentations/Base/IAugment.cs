using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 증강 기능을 수행하기 위한 인터페이스입니다.
/// </summary>
public interface IAugment
{
    /// <summary>
    /// 무기 자체의 스탯(공격 속도 등)을 수정합니다.
    /// </summary>
    /// <param name="context">무기 스탯 컨텍스트</param>
    void ModifyWeapon(WeaponContext context);

    /// <summary>
    /// 무기 발사 시 총알 데이터를 수정하거나 추가합니다.
    /// </summary>
    /// <param name="context">발사 관련 컨텍스트 데이터</param>
    void ModifyFire(FireContext context);

    /// <summary>
    /// 플레이어의 스탯(체력, 이동 속도 등)을 수정합니다.
    /// </summary>
    /// <param name="context">플레이어 스탯 컨텍스트</param>
    void ModifyPlayer(PlayerContext context);
}

/// <summary>
/// 무기 자체의 기본 스탯을 담는 컨텍스트 클래스입니다.
/// </summary>
public class WeaponContext
{
    public float fireRateMultiplier = 1.0f;
    public float damageMultiplier = 1.0f;
    public float bulletSpeedMultiplier = 1.0f;
}

/// <summary>
/// 플레이어 자체의 스탯을 담는 컨텍스트 클래스입니다.
/// </summary>
public class PlayerContext
{
    public float maxHealthMultiplier = 1.0f;
    public float moveSpeedMultiplier = 1.0f;
    public float expGainMultiplier = 1.0f;
    public float damageMultiplier = 1.0f; // 모든 무기에 적용되는 데미지 배율

    // 필살기 전용 배율 (기본 1.0)
    public float ultimateDamageMultiplier = 1.0f;
    public float ultimateFireRateMultiplier = 1.0f;
}

/// <summary>
/// 무기 발사 시 생성될 총알들의 정보를 담는 컨텍스트 클래스입니다.
/// </summary>
public class FireContext
{
    public List<BulletData> bullets = new List<BulletData>();
}

/// <summary>
/// 개별 총알의 발사 정보를 담는 데이터 구조체입니다.
/// </summary>
[System.Serializable]
public struct BulletData
{
    public Vector2 direction;
    public int damage;
    public float speed;
    public int pierceCount;
    public Sprite bulletSprite;
    public Vector3 scale;

    // 분열 관련 데이터 추가
    public int splitCount;          // 분열 시 생성될 탄환 개수
    public float splitDamageMultiplier; // 분열된 탄환의 데미지 배율
    public float splitScaleMultiplier;  // 분열된 탄환의 크기 배율

    // 도탄 관련 데이터 추가
    public int bounceCount;         // 도탄 가능 횟수
    
    // 다중 발사 관련 데이터 추가 (발사 횟수)
    public int multiShotCount;

    // 기본값이 설정된 BulletData를 반환하는 정적 속성
    public static BulletData Default => new BulletData
    {
        direction = Vector2.up,
        damage = 10,
        speed = 20f,
        scale = Vector3.one,
        splitCount = 0,
        splitDamageMultiplier = 0.5f,
        splitScaleMultiplier = 0.5f,
        bounceCount = 0,
        multiShotCount = 0
    };
    
    public BulletData(Vector2 direction, int damage, float speed, Vector3 baseScale, Sprite sprite = null)
    {
        this.direction = direction;
        this.damage = damage;
        this.speed = speed;
        this.pierceCount = 0;
        this.bulletSprite = sprite;
        this.scale = baseScale;
        this.splitCount = 0;
        this.splitDamageMultiplier = 0.5f;
        this.splitScaleMultiplier = 0.5f;
        this.bounceCount = 0;
        this.multiShotCount = 0;
    }
}
