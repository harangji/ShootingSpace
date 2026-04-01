using UnityEngine;

/// <summary>
/// 탄환을 세 방향으로 발사하게 만드는 무기 고유 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "TripleShot", menuName = "ShootingSpace/Augments/Weapon/TripleShot")]
public class TripleShotSO : WeaponAugmentSO
{
    [Header("Triple Shot Settings")]
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private float damageMultiplier = 0.7f;

    public override void ModifyWeapon(WeaponContext context)
    {
        // 공격 속도나 데미지 배율을 조정할 수 있습니다.
        context.damageMultiplier *= damageMultiplier;
    }

    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets.Count == 0) return;

        // 기본 탄환 데이터 추출
        BulletData original = context.bullets[0];
        context.bullets.Clear();

        // 왼쪽, 중앙, 오른쪽 세 방향의 탄환 데이터 생성
        float[] angles = { -spreadAngle, 0f, spreadAngle };

        foreach (float angle in angles)
        {
            BulletData newBullet = original;
            
            // 방향 회전 적용
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            newBullet.direction = rotation * original.direction;
            
            // 원본의 데이터를 그대로 복사합니다.
            context.bullets.Add(newBullet);
        }
    }
}
