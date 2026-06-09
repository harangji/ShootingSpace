using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 추가 탄환을 발사하게 만드는 무기 고유 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "AddPlanet", menuName = "ShootingSpace/Augments/Weapon/AddPlanet")]
public class AddPlanetSO : WeaponAugmentSO
{
    protected override void OnEnable()
    {
        base.OnEnable();
        maxLevel = 3;
    }
    
    public override string GetDescription()
    {
        int currentCount = 3 + level;
        int nextCount = 3 + level + 1;
        string lvText = IsMaxLevel ? "최대 레벨" : $"Lv.{level} -> Lv.{level + 1}";
        string nextEffect = IsMaxLevel ? "" : $"(다음: {nextCount}개 회전)";
        return $"{description}[현재: {currentCount}개 회전] <{lvText}>{nextEffect}";
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
        bullet.multiShotCount = 2 + level;
        bullet.multiShotInterval = bullet.speed / (bullet.multiShotCount + 1);
        context.bullets.Add(bullet);
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

}
