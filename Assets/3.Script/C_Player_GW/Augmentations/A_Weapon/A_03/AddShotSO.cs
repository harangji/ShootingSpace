using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 추가 탄환을 발사하게 만드는 무기 고유 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "AddShot", menuName = "ShootingSpace/Augments/Weapon/AddShot")]
public class AddShotSO : WeaponAugmentSO
{
    protected override void OnEnable()
    {
        base.OnEnable();
        maxLevel = 4;
    }

    [Title("추가 탄환 설정")]
    [LabelText("확산 각도"), Range(5f, 45f)]
    [SerializeField] private float spreadAngle = 15f;

    [LabelText("데미지 배율"), Range(0.1f, 1.0f)]
    [Tooltip("탄환 수가 늘어나는 대신 개별 데미지가 줄어듭니다.")]
    [SerializeField] private float damageMultiplier = 0.8f;
    
    public override string GetDescription()
    {
        int currentCount = 1 + level;
        int nextCount = 1 + level + 1;
        string lvText = IsMaxLevel ? "최대 레벨" : $"Lv.{level} -> Lv.{level + 1}";
        string nextEffect = IsMaxLevel ? "" : $"\n(다음: {nextCount}발 발사)";

        return $"{description}\n[현재: {currentCount}발 발사]\n<{lvText}>{nextEffect}";
    }

    public override void ModifyWeapon(WeaponContext context)
    {
        context.damageMultiplier *= damageMultiplier;
    }

    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets.Count == 0) return;

        BulletData original = context.bullets[0];
        context.bullets.Clear();

        int bulletCount = level + 1; // Level 1 = 2 shots, Level 2 = 3 shots, etc.

        // 총알을 균등하게 분산시키기 위한 각도 계산
        float startAngle = -(bulletCount - 1) * spreadAngle * 0.5f;

        for (int i = 0; i < bulletCount; i++)
        {
            BulletData newBullet = original;
            float currentAngle = startAngle + i * spreadAngle;
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            newBullet.direction = rotation * original.direction;
            context.bullets.Add(newBullet);
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

}
