using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 적중 시 탄환이 근처의 다른 적에게 튕겨 나가게 만드는 무기 고유 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "RicochetShot", menuName = "ShootingSpace/Augments/Weapon/RicochetShot")]
public class RicochetShotSO : WeaponAugmentSO
{
    [Title("도탄 설정")]
    [LabelText("도탄 횟수"), PropertyRange(1, 10)]
    [Tooltip("탄환이 근처 적에게 튕기는 최대 횟수입니다.")]
    [SerializeField] private int bounceCount = 2;

    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets == null) return;

        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            bullet.bounceCount = bounceCount;
            context.bullets[i] = bullet;
        }
    }
}
