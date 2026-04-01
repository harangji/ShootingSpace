using UnityEngine;

/// <summary>
/// 탄환을 대포알로 변경하는 고유 증강입니다.
/// 외형이 커지고 데미지가 증가하지만 탄속이 느려집니다.
/// </summary>
[CreateAssetMenu(fileName = "CannonBall", menuName = "ShootingSpace/Augments/Unique/CannonBall")]
public class CannonBallSO : WeaponAugmentSO
{
    [Header("Cannon Settings")]
    public Sprite cannonSprite;
    public float scaleMultiplier = 2.5f;
    public float damageMultiplier = 3.0f;
    public float speedMultiplier = 0.5f;

    public override void ModifyFire(FireContext context)
    {
        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            if (cannonSprite != null) bullet.bulletSprite = cannonSprite;
            bullet.scale *= scaleMultiplier;
            bullet.damage = Mathf.RoundToInt(bullet.damage * damageMultiplier);
            bullet.speed *= speedMultiplier;
            context.bullets[i] = bullet;
        }
    }
}
