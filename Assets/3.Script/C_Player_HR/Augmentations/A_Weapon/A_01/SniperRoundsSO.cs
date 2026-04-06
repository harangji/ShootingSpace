using UnityEngine;

/// <summary>
/// 탄환의 공격력과 속도를 대폭 높이는 대신 연사 속도를 낮추는 정밀 사격 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "SniperRounds", menuName = "ShootingSpace/Augments/Weapon/SniperRounds")]
public class SniperRoundsSO : WeaponAugmentSO
{
    [Header("Sniper Settings")]
    [Tooltip("공격력 배율입니다. (기본 2배)")]
    [SerializeField] private float damageMultiplier = 2.0f;
    
    [Tooltip("탄환 속도 배율입니다. (기본 1.5배)")]
    [SerializeField] private float speedMultiplier = 1.5f;
    
    [Tooltip("공격 속도 배율입니다. (낮을수록 느려짐, 기본 0.7배)")]
    [SerializeField] private float fireRateMultiplier = 0.7f;

    /// <summary>
    /// 무기의 기본 능력치를 수정합니다.
    /// </summary>
    public override void ModifyWeapon(WeaponContext context)
    {
        context.damageMultiplier *= damageMultiplier;
        context.bulletSpeedMultiplier *= speedMultiplier;
        context.fireRateMultiplier *= fireRateMultiplier;
    }

    /// <summary>
    /// 발사되는 탄환의 데이터를 수정합니다.
    /// 여기서는 추가적인 탄환 데이터 수정이 필요 없으므로 기본 구현을 유지하거나 
    /// 필요한 경우 탄환의 크기를 조정할 수 있습니다.
    /// </summary>
    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets == null) return;

        // 저격 탄환의 느낌을 주기 위해 탄환의 크기를 약간 길쭉하게 만들 수도 있습니다.
        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            bullet.scale = new Vector3(bullet.scale.x * 0.8f, bullet.scale.y * 1.5f, bullet.scale.z);
            context.bullets[i] = bullet;
        }
    }
}
