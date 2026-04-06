using UnityEngine;

/// <summary>
/// 적중 시 탄환이 근처의 다른 적에게 튕겨 나가게 만드는 무기 고유 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "RicochetShot", menuName = "ShootingSpace/Augments/Weapon/RicochetShot")]
public class RicochetShotSO : WeaponAugmentSO
{
    [Header("Ricochet Settings")]
    [Tooltip("탄환이 도탄되는 횟수(n)입니다.")]
    [SerializeField] private int bounceCount = 2;

    /// <summary>
    /// 발사되는 모든 탄환에 도탄 횟수를 설정합니다.
    /// </summary>
    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets == null) return;

        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            
            // 도탄 횟수 설정
            bullet.bounceCount = bounceCount;
            
            context.bullets[i] = bullet;
        }
    }
}
