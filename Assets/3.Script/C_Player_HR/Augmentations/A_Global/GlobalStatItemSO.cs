using UnityEngine;

/// <summary>
/// 모든 무기에 적용되는 전역 아이템 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "GlobalStatItem", menuName = "ShootingSpace/Augments/Global/StatItem")]
public class GlobalStatItemSO : ItemAugmentSO
{
    [Header("Global Stat Boost")]
    public float fireRateBoost = 1.0f; 
    public float damageBoost = 1.0f;   
    public float scaleBoost = 1.0f;    // 탄환 크기 배율

    [Header("Player Stat Boost")]
    public float healthMultiplier = 1.0f;
    public float moveSpeedMultiplier = 1.0f;
    public float expMultiplier = 1.0f;
    
    // 무기 자체 스탯 수정
    public override void ModifyWeapon(WeaponContext context)
    {
        context.fireRateMultiplier *= fireRateBoost;
        context.damageMultiplier *= damageBoost;
    }

    // 플레이어 스탯 수정
    public override void ModifyPlayer(PlayerContext context)
    {
        context.maxHealthMultiplier *= healthMultiplier;
        context.moveSpeedMultiplier *= moveSpeedMultiplier;
        context.expGainMultiplier *= expMultiplier;
        context.damageMultiplier *= damageBoost; // 모든 무기에 공통 데미지 증가 적용
    }

    // 발사 시 탄환 개별 데이터(크기) 수정
    public override void ModifyFire(FireContext context) 
    {
        if (context == null || context.bullets == null) return;

        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            bullet.scale *= scaleBoost; // 모든 탄환 크기 업그레이드
            context.bullets[i] = bullet;
        }
    }
}
