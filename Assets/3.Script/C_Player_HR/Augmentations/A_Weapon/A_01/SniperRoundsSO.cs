using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 탄환의 공격력과 속도를 대폭 높이는 대신 연사 속도를 낮추는 정밀 사격 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "SniperRounds", menuName = "ShootingSpace/Augments/Weapon/SniperRounds")]
public class SniperRoundsSO : WeaponAugmentSO
{
    [Title("저격 사격 설정")]
    [LabelText("공격력 배율"), Range(1.0f, 5.0f)]
    [SerializeField] private float damageMultiplier = 2.0f;
    
    [LabelText("탄환 속도 배율"), Range(1.0f, 3.0f)]
    [SerializeField] private float speedMultiplier = 1.5f;
    
    [LabelText("연사 속도 배율"), Range(0.1f, 1.0f)]
    [Tooltip("낮을수록 공격 간격이 길어집니다.")]
    [SerializeField] private float fireRateMultiplier = 0.7f;

    public override string GetDescription()
    {
        float currentDmg = (1f + (damageMultiplier - 1f) * level);
        float currentSpeed = (1f + (speedMultiplier - 1f) * level);
        
        string lvText = IsMaxLevel ? "최대 레벨" : $"Lv.{level} -> Lv.{level + 1}";
        return $"{description}\n[현재: 데미지 +{Mathf.RoundToInt((currentDmg-1f)*100f)}%, 탄속 +{Mathf.RoundToInt((currentSpeed-1f)*100f)}%]\n<{lvText}>";
    }

    public override void ModifyWeapon(WeaponContext context)
    {
        context.damageMultiplier *= (1f + (damageMultiplier - 1f) * level);
        context.bulletSpeedMultiplier *= (1f + (speedMultiplier - 1f) * level);
        // 연사 속도는 낮아지는 페널티이므로 레벨에 따라 더 심해지도록 설정
        context.fireRateMultiplier *= Mathf.Max(0.1f, 1f + (fireRateMultiplier - 1f) * level);
    }

    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets == null) return;

        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            // 길쭉한 저격탄 느낌 적용
            bullet.scale = new Vector3(bullet.scale.x * 0.8f, bullet.scale.y * 1.5f, bullet.scale.z);
            context.bullets[i] = bullet;
        }
    }
}
