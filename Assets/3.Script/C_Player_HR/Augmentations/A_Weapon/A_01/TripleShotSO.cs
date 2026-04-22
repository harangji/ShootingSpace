using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 탄환을 세 방향으로 발사하게 만드는 무기 고유 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "TripleShot", menuName = "ShootingSpace/Augments/Weapon/TripleShot")]
public class TripleShotSO : WeaponAugmentSO
{
    [Title("삼중 사격 설정")]
    [LabelText("확산 각도"), Range(5f, 45f)]
    [SerializeField] private float spreadAngle = 15f;

    [LabelText("데미지 배율"), Range(0.1f, 1.0f)]
    [Tooltip("탄환 수가 늘어나는 대신 개별 데미지가 줄어듭니다.")]
    [SerializeField] private float damageMultiplier = 0.7f;

    public override string GetDescription()
    {
        int currentCount = 2 + level;
        int nextCount = 2 + level + 1;
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

        // 레벨 1일 때 3발, 레벨 2일 때 4발... (n + 1 로직 적용)
        int bulletCount = 2 + level;
        
        // 전체 퍼짐 정도를 계산하여 각 탄환의 각도를 결정
        // 예: 3발일 경우 -spreadAngle, 0, +spreadAngle
        float startAngle = -(bulletCount - 1) * spreadAngle * 0.5f;

        for (int i = 0; i < bulletCount; i++)
        {
            BulletData newBullet = original;
            float currentAngle = startAngle + (i * spreadAngle);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            newBullet.direction = rotation * original.direction;
            context.bullets.Add(newBullet);
        }
    }
}
