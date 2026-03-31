using UnityEngine;

/// <summary>
/// 모든 무기에 적용되는 전역 아이템 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "GlobalStatItem", menuName = "ShootingSpace/Augments/Global/StatItem")]
public class GlobalStatItemSO : AugmentSO
{
    [Header("Global Stat Boost")]
    public float fireRateBoost = 1.5f; 
    public float damageBoost = 1.5f;   
    public float scaleBoost = 1.2f;    // 탄환 크기 배율 추가!
    
    // 무기 자체 스탯 수정
    public override void ModifyWeapon(WeaponContext context)
    {
        context.fireRateMultiplier *= fireRateBoost;
        context.damageMultiplier *= damageBoost;
    }

    // 발사 시 탄환 개별 데이터(크기) 수정
    public override void ModifyFire(FireContext context) 
    {
        if (context == null || context.bullets == null) return;

        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            bullet.scale *= scaleBoost; // 모든 탄환 크기 업그레이드!
            context.bullets[i] = bullet;
        }
    }
}
