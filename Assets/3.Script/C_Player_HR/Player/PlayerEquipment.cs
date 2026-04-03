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
    /// 전역 아이템 증강을 장착하거나 레벨업합니다.
    /// </summary>
    public bool EquipItem(ItemAugmentSO augment)
    {
        if (augment == null) return false;

        // 1. 이미 가지고 있는지 확인
        ItemAugmentSO existing = itemSlots.Find(i => i != null && i.augmentName == augment.augmentName);
        
        if (existing != null)
        {
            // 이미 있으면 레벨업 (최대 레벨 체크는 Selector에서 수행함)
            existing.LevelUp();
            Debug.Log($"[Equipment] {existing.augmentName} 레벨업! (Lv.{existing.level})");
            return true;
        }

        // 2. 새로 장착 (빈 슬롯 찾기)
        int emptyIndex = itemSlots.FindIndex(i => i == null);
        if (emptyIndex == -1)
        {
            Debug.LogWarning("[Equipment] 아이템 슬롯이 가득 찼습니다.");
            return false;
        }

        // 원본 에셋 보호를 위해 인스턴스화하여 장착
        ItemAugmentSO instance = Instantiate(augment);
        itemSlots[emptyIndex] = instance;
        Debug.Log($"[Equipment] {instance.augmentName} 신규 장착!");
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
            ItemAugmentSO target = itemSlots[index];
            itemSlots[index] = null;
            if (target != null) Destroy(target); // 인스턴스 파괴
            return true;
        }
        return false;
    }

    /// <summary>
    /// 전역 아이템 증강이 장착되어 있는지 확인하고, 있다면 해당 인스턴스를 반환합니다.
    /// </summary>
    public ItemAugmentSO GetItem(string augmentName)
    {
        return itemSlots.Find(i => i != null && i.augmentName == augmentName);
    }

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

        // 지정된 mountPoint(보통 플레이어)의 자식으로 생성
        WeaponBase newWeapon = Instantiate(weaponPrefab, mountPoint);
        
        // 위치 및 회전 초기화하여 플레이어에게 딱 붙게 함
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        weaponSlots[emptyIndex] = newWeapon;
        return true;
    }
}
