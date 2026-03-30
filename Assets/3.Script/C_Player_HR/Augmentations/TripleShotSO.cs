using UnityEngine;

/// <summary>
/// 에셋 메뉴를 통해 생성 가능한 세 갈래 발사 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "TripleShot", menuName = "ShootingSpace/Augments/TripleShot")]
public class TripleShotSO : AugmentSO
{
    [Header("발사 설정")]
    [Range(0, 45)] public float spreadAngle = 15f;

    public override void ModifyFire(FireContext context)
    {
        if (context.bullets.Count == 0) return;

        // 원본 탄환 정보 (정면)
        BulletData original = context.bullets[0];
        
        context.bullets.Add(Create(original, spreadAngle));
        context.bullets.Add(Create(original, -spreadAngle));
    }
    
    /// <summary>
    /// 원본 탄환 데이터를 바탕으로 지정된 각도만큼 회전된 새로운 탄환 데이터를 생성합니다.
    /// </summary>
    /// <param name="origin">원본 탄환 데이터</param>
    /// <param name="angle">회전할 각도 (Z축 기준)</param>
    /// <returns>회전된 방향을 가진 새로운 탄환 데이터</returns>
    private BulletData Create(BulletData origin, float angle)
    {
        // 2D 환경에서는 Z축(Vector3.forward)을 기준으로 회전해야 평면상에서 방향이 변합니다.
        Vector2 rotatedDirection = Quaternion.Euler(0, 0, angle) * origin.direction;

        return new BulletData
        {
            direction = rotatedDirection,
            damage = origin.damage,
            speed = origin.speed,
            pierceCount = origin.pierceCount
        };
    }
}
