using UnityEngine;

/// <summary>
/// 적중 시 탄환이 뒤로(진행 방향의 부채꼴로) 분열되게 만드는 무기 고유 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "SplitShot", menuName = "ShootingSpace/Augments/Weapon/SplitShot")]
public class SplitShotSO : WeaponAugmentSO
{
    [Header("Split Settings")]
    [Tooltip("분열될 탄환의 개수입니다.")]
    [SerializeField] private int splitCount = 2;
    
    [Tooltip("분열된 탄환의 데미지 배율입니다.")]
    [SerializeField] private float damageMultiplier = 0.3f;
    
    [Tooltip("분열된 탄환의 크기 배율입니다.")]
    [SerializeField] private float scaleMultiplier = 0.3f;

    public override string GetDescription()
    {
        int current = splitCount * level;
        int next = splitCount * (level + 1);
        string lvText = IsMaxLevel ? "최대 레벨" : $"Lv.{level} -> Lv.{level + 1}";
        string nextEffect = IsMaxLevel ? "" : $"\n(다음: {next}개 분열)";
        
        return $"{description}\n[현재: {current}개 분열]\n<{lvText}>{nextEffect}";
    }

    /// <summary>
    /// 발사되는 모든 탄환에 분열 데이터를 심어줍니다.
    /// </summary>
    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets == null) return;

        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            
            // 분열 정보 주입 (레벨당 분열 개수 증가)
            bullet.splitCount = splitCount * level;
            bullet.splitDamageMultiplier = damageMultiplier;
            bullet.splitScaleMultiplier = scaleMultiplier;
            
            context.bullets[i] = bullet;
        }
    }
}
