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

    // 기본값이 설정된 BulletData를 반환하는 정적 속성
    public static BulletData Default => new BulletData
    {
        direction = Vector2.up,
        damage = 10,
        speed = 20f,
        scale = Vector3.one
    };
// 기본값 설정을 위한 생성자
public BulletData(Vector2 direction, int damage, float speed, Vector3 baseScale, Sprite sprite = null)
{
    this.direction = direction;
    this.damage = damage;
    this.speed = speed;
    this.pierceCount = 0;
    this.bulletSprite = sprite;
    this.scale = baseScale; // 원본 스케일을 보존한다냥!
}
}

