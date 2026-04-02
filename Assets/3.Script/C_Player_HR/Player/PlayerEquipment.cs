using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector; // 오딘 네임스페이스 추가!

/// <summary>
/// 플레이어의 무기 및 아이템 슬롯을 관리하고 장착 기능을 제공하는 클래스입니다.
/// </summary>
public class PlayerEquipment : MonoBehaviour
{
    [Title("Slot Settings")]
    [HorizontalGroup("WeaponSlotsControl")]
    [PropertyRange(1, 8)] // 슬라이더 조절로 변경 (Unity 6 드롭다운 오류 회피!)
    [OnValueChanged("SyncWeaponSlots")]
    [LabelText("최대 무기 슬롯")]
    [SerializeField] private int maxWeaponSlots = 3;

    [HorizontalGroup("WeaponSlotsControl", Width = 0.05f)]
    [Button(SdfIconType.Dash, Name = "")]
    private void SubWeapon() { maxWeaponSlots = Mathf.Max(1, maxWeaponSlots - 1); SyncWeaponSlots(); }

    [HorizontalGroup("WeaponSlotsControl", Width = 0.05f)]
    [Button(SdfIconType.Plus, Name = "")]
    private void AddWeapon() { maxWeaponSlots = Mathf.Min(8, maxWeaponSlots + 1); SyncWeaponSlots(); }

    [HorizontalGroup("ItemSlotsControl")]
    [PropertyRange(1, 20)]
    [OnValueChanged("SyncItemSlots")]
    [LabelText("최대 아이템 슬롯")]
    [SerializeField] private int maxItemSlots = 5;

    [HorizontalGroup("ItemSlotsControl", Width = 0.05f)]
    [Button(SdfIconType.Dash, Name = "")]
    private void SubItem() { maxItemSlots = Mathf.Max(1, maxItemSlots - 1); SyncItemSlots(); }

    [HorizontalGroup("ItemSlotsControl", Width = 0.05f)]
    [Button(SdfIconType.Plus, Name = "")]
    private void AddItem() { maxItemSlots = Mathf.Min(20, maxItemSlots + 1); SyncItemSlots(); }

    [Title("Current Equipment")]
    [ListDrawerSettings(IsReadOnly = true, ShowIndexLabels = true)]
    [SerializeField] private List<WeaponBase> weaponSlots = new List<WeaponBase>();

    [ListDrawerSettings(IsReadOnly = true, ShowIndexLabels = true)]
    [SerializeField] private List<ItemAugmentSO> itemSlots = new List<ItemAugmentSO>();

    public List<WeaponBase> WeaponSlots => weaponSlots;
    public List<ItemAugmentSO> ItemSlots => itemSlots;

    private void OnValidate()
    {
        // 에디터 초기 로드 시에도 크기를 맞춰줍니다.
        SyncWeaponSlots();
        SyncItemSlots();
    }

    private void SyncWeaponSlots() => AdjustListSize(weaponSlots, maxWeaponSlots);
    private void SyncItemSlots() => AdjustListSize(itemSlots, maxItemSlots);

    private void AdjustListSize<T>(List<T> list, int size)
    {
        if (list == null) return;

        while (list.Count < size) list.Add(default);
        while (list.Count > size) list.RemoveAt(list.Count - 1);
    }

    /// <summary>
    /// 초기 무기 설정 및 장착 장치 초기화를 수행합니다.
    /// </summary>
    public void Initialize()
    {
        WeaponBase[] existingWeapons = GetComponentsInChildren<WeaponBase>();
        for (int i = 0; i < existingWeapons.Length && i < maxWeaponSlots; i++)
        {
            if (weaponSlots[i] == null)
            {
                weaponSlots[i] = existingWeapons[i];
            }
        }
    }

    /// <summary>
    /// 전역 아이템 증강을 장착합니다.
    /// </summary>
    public bool EquipItem(ItemAugmentSO augment)
    {
        if (augment == null) return false;

        int emptyIndex = itemSlots.FindIndex(i => i == null);
        if (emptyIndex == -1)
        {
            Debug.LogWarning("[Equipment] 아이템 슬롯이 가득 찼습니다.");
            return false;
        }

        itemSlots[emptyIndex] = augment;
        return true;
    }

    /// <summary>
    /// 전역 아이템 증강을 해제합니다.
    /// </summary>
    public bool UnequipItem(string augmentName)
    {
        int index = itemSlots.FindIndex(i => i != null && i.augmentName == augmentName);
        if (index != -1)
        {
            itemSlots[index] = null;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 전역 아이템 증강이 장착되어 있는지 확인합니다.
    /// </summary>
    public bool HasItem(string augmentName)
    {
        return itemSlots.Exists(i => i != null && i.augmentName == augmentName);
    }

    /// <summary>
    /// 새로운 무기를 장착합니다.
    /// </summary>
    public bool EquipWeapon(WeaponBase weaponPrefab, Transform mountPoint)
    {
        if (weaponPrefab == null) return false;

        int emptyIndex = weaponSlots.FindIndex(w => w == null);
        if (emptyIndex == -1)
        {
            Debug.LogWarning("[Equipment] 무기 슬롯이 가득 찼습니다.");
            return false;
        }

        WeaponBase newWeapon = Instantiate(weaponPrefab, mountPoint);
        weaponSlots[emptyIndex] = newWeapon;
        return true;
    }
}
