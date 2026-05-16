using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 무기 공격력을 증가시키는 무기 고유 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "AddDamage", menuName = "ShootingSpace/Augments/Weapon/AddDamage")]
public class AddDamageSO : WeaponAugmentSO
{
    protected override void OnEnable()
    {
        base.OnEnable();
        maxLevel = 4;
    }

    [Title("공격력 증가 설정")]
    [LabelText("레벨당 공격력 배율 증가"), Range(0.01f, 0.5f)]
    [SerializeField] private float damageMultiplierIncreasePerLevel = 0.1f; // 레벨당 10% 증가

    public override void ModifyWeapon(WeaponContext context)
    {
        // 현재 레벨에 비례하여 공격력 배율을 증가시킵니다.
        // level은 1부터 시작하므로, level 1일 때 damageMultiplierIncreasePerLevel * 1 이 됩니다.
        context.damageMultiplier *= (1 + (damageMultiplierIncreasePerLevel * level));
    }

    public override void ModifyFire(FireContext context)
    {
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    public override string GetDescription()
    {
        // 증강 레벨에 따른 동적인 설명을 제공합니다.
        float currentIncrease = (damageMultiplierIncreasePerLevel * level) * 100;
        return $"{description} <color=yellow>현재 레벨 {level} : 공격력 +{currentIncrease:F0}%</color>";
    }
}
