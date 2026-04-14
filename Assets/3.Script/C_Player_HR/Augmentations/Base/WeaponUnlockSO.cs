using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 새로운 무기를 해금하고 장착할 수 있게 해주는 증강 타입입니다.
/// </summary>
[CreateAssetMenu(fileName = "WeaponUnlock", menuName = "ShootingSpace/Augments/WeaponUnlock")]
public class WeaponUnlockSO : AugmentSO
{
    [Title("신규 무기 설정")]
    [LabelText("무기 프리팹"), AssetSelector(Paths = "Assets/9.Prefab/HR/Weapon")]
    [Required("장착할 무기 프리팹이 필요합니다.")]
    public WeaponBase weaponPrefab;

    private void OnEnable()
    {
        type = AugmentType.NewWeapon;
    }

    public override void ModifyFire(FireContext context) { }
}
