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
    
    public override string GetDescription()
    {
        string currentStats = "";
        if (fireRateBoost != 1.0f) currentStats += $"\n- 연사 속도: +{Mathf.RoundToInt((fireRateBoost - 1f) * level * 100f)}%";
        if (damageBoost != 1.0f) currentStats += $"\n- 데미지: +{Mathf.RoundToInt((damageBoost - 1f) * level * 100f)}%";
        if (scaleBoost != 1.0f) currentStats += $"\n- 탄환 크기: +{Mathf.RoundToInt((scaleBoost - 1f) * level * 100f)}%";
        if (moveSpeedMultiplier != 1.0f) currentStats += $"\n- 이동 속도: +{Mathf.RoundToInt((moveSpeedMultiplier - 1f) * level * 100f)}%";
        
        string lvText = IsMaxLevel ? "최대 레벨" : $"Lv.{level} -> Lv.{level + 1}";
        return $"{description}{currentStats}\n<{lvText}>";
    }

    public override void ModifyWeapon(WeaponContext context)
    {
        context.fireRateMultiplier *= (1f + (fireRateBoost - 1f) * level);
        context.damageMultiplier *= (1f + (damageBoost - 1f) * level);
    }

    public override void ModifyPlayer(PlayerContext context)
    {
        context.maxHealthMultiplier *= (1f + (healthMultiplier - 1f) * level);
        context.moveSpeedMultiplier *= (1f + (moveSpeedMultiplier - 1f) * level);
        context.expGainMultiplier *= (1f + (expMultiplier - 1f) * level);
        context.damageMultiplier *= (1f + (damageBoost - 1f) * level);
    }

    public override void ModifyFire(FireContext context) 
    {
        if (context == null || context.bullets == null) return;

        float effectiveScaleBoost = (1f + (scaleBoost - 1f) * level);
        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            bullet.scale *= effectiveScaleBoost;
            context.bullets[i] = bullet;
        }
    }
}
