using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 추가 탄환을 발사하게 만드는 무기 고유 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "SpinSpeed", menuName = "ShootingSpace/Augments/Weapon/SpinSpeed")]
public class SpinSpeedSO : WeaponAugmentSO
{
    protected override void OnEnable()
    {
        base.OnEnable();
        maxLevel = 2;
    }
    
    public override string GetDescription()
    {
        float currentCount = 0.5f * level;
        float nextCount = 0.5f * (level + 1);
        string lvText = IsMaxLevel ? "최대 레벨" : $"Lv.{level} -> Lv.{level + 1}";
        string nextEffect = IsMaxLevel ? "" : $"(다음: {nextCount}초 증가)";
        return $"{description}[현재: {currentCount}초 증가] <{lvText}>{nextEffect}";
    }

    public override void ModifyWeapon(WeaponContext context)
    {
    }

    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets.Count == 0) return;

        BulletData original = context.bullets[0];
        context.bullets.Clear();

        BulletData bullet = original;
        bullet.spinSpeed = 2.0f - (level * 0.5f);
        bullet.multiShotInterval = bullet.spinSpeed / (bullet.multiShotCount + 1);
        context.bullets.Add(bullet);
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

}
