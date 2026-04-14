using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 모든 무기에 적용되는 전역 아이템 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "GlobalStatItem", menuName = "ShootingSpace/Augments/Global/StatItem")]
public class GlobalStatItemSO : ItemAugmentSO
{
    [Title("전역 무기 강화 배율")]
    [LabelText("연사 속도 보너스"), Tooltip("1.0 이 기본입니다.")]
    public float fireRateBoost = 1.0f; 
    [LabelText("데미지 보너스")]
    public float damageBoost = 1.0f;   
    [LabelText("탄환 크기 배율")]
    public float scaleBoost = 1.0f;

    [Title("플레이어 강화 배율")]
    [LabelText("최대 체력 배율")]
    public float healthMultiplier = 1.0f;
    [LabelText("이동 속도 배율")]
    public float moveSpeedMultiplier = 1.0f;
    [LabelText("경험치 획득 배율")]
    public float expMultiplier = 1.0f;
    
    public override void ModifyWeapon(WeaponContext context)
    {
        context.fireRateMultiplier *= fireRateBoost;
        context.damageMultiplier *= damageBoost;
    }

    public override void ModifyPlayer(PlayerContext context)
    {
        context.maxHealthMultiplier *= healthMultiplier;
        context.moveSpeedMultiplier *= moveSpeedMultiplier;
        context.expGainMultiplier *= expMultiplier;
        context.damageMultiplier *= damageBoost;
    }

    public override void ModifyFire(FireContext context) 
    {
        if (context == null || context.bullets == null) return;

        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            bullet.scale *= scaleBoost;
            context.bullets[i] = bullet;
        }
    }
}
