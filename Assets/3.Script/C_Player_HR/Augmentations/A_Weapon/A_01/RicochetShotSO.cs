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

    public override string GetDescription()
    {
        int current = bounceCount * level;
        int next = bounceCount * (level + 1);
        string lvText = IsMaxLevel ? "최대 레벨" : $"Lv.{level} -> Lv.{level + 1}";
        string nextEffect = IsMaxLevel ? "" : $"\n(다음: {next}회 도탄)";
        
        return $"{description}\n[현재: {current}회 도탄]\n<{lvText}>{nextEffect}";
    }

    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets == null) return;

        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            // 레벨에 비례하여 도탄 횟수 설정
            bullet.bounceCount = bounceCount * level;
            context.bullets[i] = bullet;
        }
    }
}
