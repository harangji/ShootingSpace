using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 증강 기능을 수행하기 위한 인터페이스입니다.
/// </summary>
public interface IAugment
{
    /// <summary>
    /// 무기 발사 시 총알 데이터를 수정하거나 추가합니다.
    /// </summary>
    /// <param name="context">발사 관련 컨텍스트 데이터</param>
    void ModifyFire(FireContext context);
}

/// <summary>
/// 무기 발사 시 생성될 총알들의 정보를 담는 컨텍스트 클래스입니다.
/// </summary>
public class FireContext
{
    public List<BulletData> bullets = new List<BulletData>();
}

/// <summary>
/// 개별 총알의 발사 정보를 담는 데이터 클래스입니다.
/// </summary>
public class BulletData
{
    public Vector2 direction;
    public int damage;
    public float speed;
    public int pierceCount;
}
