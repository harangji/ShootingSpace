using UnityEngine;

/// <summary>
/// 새로운 무기를 해금하고 장착할 수 있게 해주는 증강 타입입니다.
/// </summary>
[CreateAssetMenu(fileName = "WeaponUnlock", menuName = "ShootingSpace/Augments/WeaponUnlock")]
public class WeaponUnlockSO : AugmentSO
{
    [Header("New Weapon")]
    public WeaponBase weaponPrefab; // 장착할 무기 프리팹

    private void OnEnable()
    {
        type = AugmentType.NewWeapon;
    }

    // 무기 획득 시 발사 로직 수정은 필요 없으므로 빈 구현
    public override void ModifyFire(FireContext context) { }
}
