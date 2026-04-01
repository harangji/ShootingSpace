using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 플레이어의 무기 및 아이템 슬롯을 관리하고 장착 기능을 제공하는 클래스입니다.
/// </summary>
public class PlayerEquipment : MonoBehaviour
{
    [Header("Slot Settings")]
    [SerializeField] private int maxWeaponSlots = 3;
    [SerializeField] private int maxItemSlots = 5;

    [Header("Current Equipment")]
    [SerializeField] private List<WeaponBase> weaponSlots = new List<WeaponBase>();
    [SerializeField] private List<ItemAugmentSO> itemSlots = new List<ItemAugmentSO>();

    public List<WeaponBase> WeaponSlots => weaponSlots;
    public List<ItemAugmentSO> ItemSlots => itemSlots;

    /// <summary>
    /// 초기 무기 설정 및 장착 장치 초기화를 수행합니다.
    /// </summary>
    public void Initialize()
    {
        if (weaponSlots.Count == 0)
        {
            weaponSlots.AddRange(GetComponentsInChildren<WeaponBase>());
        }
    }

    /// <summary>
    /// 전역 아이템 증강을 장착합니다.
    /// </summary>
    public bool EquipItem(ItemAugmentSO augment)
    {
        if (augment == null) return false;

        if (itemSlots.Count >= maxItemSlots)
        {
            Debug.LogWarning("[Equipment] 아이템 슬롯이 가득 찼습니다.");
            return false;
        }

        itemSlots.Add(augment);
        return true;
    }

    /// <summary>
    /// 전역 아이템 증강을 해제합니다.
    /// </summary>
    public bool UnequipItem(string augmentName)
    {
        ItemAugmentSO target = itemSlots.Find(i => i.augmentName == augmentName);
        if (target != null)
        {
            itemSlots.Remove(target);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 전역 아이템 증강이 장착되어 있는지 확인합니다.
    /// </summary>
    public bool HasItem(string augmentName)
    {
        return itemSlots.Exists(i => i.augmentName == augmentName);
    }

    /// <summary>
    /// 새로운 무기를 장착합니다.
    /// </summary>
    public bool EquipWeapon(WeaponBase weaponPrefab, Transform mountPoint)
    {
        if (weaponPrefab == null) return false;

        if (weaponSlots.Count >= maxWeaponSlots)
        {
            Debug.LogWarning("[Equipment] 무기 슬롯이 가득 찼습니다.");
            return false;
        }

        WeaponBase newWeapon = Instantiate(weaponPrefab, mountPoint);
        weaponSlots.Add(newWeapon);
        return true;
    }
}
