using UnityEngine;

/// <summary>
/// 전방 사격 시 후방으로도 탄환을 발사하게 만드는 무기 고유 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "RearShot", menuName = "ShootingSpace/Augments/Weapon/RearShot")]
public class RearShotSO : WeaponAugmentSO
{
    [Header("Rear Shot Settings")]
    [Tooltip("후방으로 발사되는 탄환의 데미지 배율입니다.")]
    [SerializeField] private float rearDamageMultiplier = 0.5f;
    [Tooltip("후방 탄환의 속도 배율입니다.")]
    [SerializeField] private float rearSpeedMultiplier = 1.0f;

    public override string GetDescription()
    {
        float currentDmg = rearDamageMultiplier + (level - 1) * 0.1f;
        float nextDmg = rearDamageMultiplier + level * 0.1f;
        string lvText = IsMaxLevel ? "최대 레벨" : $"Lv.{level} -> Lv.{level + 1}";
        string nextEffect = IsMaxLevel ? "" : $"\n(다음: 데미지 {Mathf.RoundToInt(nextDmg * 100f)}%)";

        return $"{description}\n[현재: 후방 데미지 {Mathf.RoundToInt(currentDmg * 100f)}%]\n<{lvText}>{nextEffect}";
    }

    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets == null || context.bullets.Count == 0) return;

        // 레벨에 따라 후방 탄환 데미지 보너스 증가 (기본 배율 + 레벨당 보너스)
        float effectiveDamageMultiplier = rearDamageMultiplier + (level - 1) * 0.1f;

        // 현재 발사하려는 탄환들의 리스트를 복사하여 후방 탄환을 생성합니다.
        int originalCount = context.bullets.Count;
        for (int i = 0; i < originalCount; i++)
        {
            BulletData original = context.bullets[i];
            BulletData rearBullet = original;

            // 방향을 정확히 반대로 설정합니다.
            rearBullet.direction = -original.direction;
            
            // 능력치 조정
            rearBullet.damage = Mathf.RoundToInt(original.damage * effectiveDamageMultiplier);
            rearBullet.speed = original.speed * rearSpeedMultiplier;

            // 리스트에 추가하여 함께 발사되도록 합니다.
            context.bullets.Add(rearBullet);
        }
    }
}
