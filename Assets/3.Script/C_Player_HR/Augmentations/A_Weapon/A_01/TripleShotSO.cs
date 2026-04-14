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

    public override void ModifyWeapon(WeaponContext context)
    {
        context.damageMultiplier *= damageMultiplier;
    }

    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets.Count == 0) return;

        BulletData original = context.bullets[0];
        context.bullets.Clear();

        float[] angles = { -spreadAngle, 0f, spreadAngle };

        foreach (float angle in angles)
        {
            BulletData newBullet = original;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            newBullet.direction = rotation * original.direction;
            context.bullets.Add(newBullet);
        }
    }
}
