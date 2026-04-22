using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 등장 가능한 증강들을 필터링하고 무작위로 선택하는 시스템입니다.
/// </summary>
public static class AugmentSelector
{
    public static List<AugmentSO> GetAvailableAugments(PlayerEquipment equipment, AugmentLibrarySO library)
    {
        if (equipment == null || library == null) return new List<AugmentSO>();

        bool hasGlobalSpace = equipment.ItemSlots.Any(slot => slot == null);
        bool hasWeaponSpace = equipment.WeaponSlots.Any(slot => slot == null);

        // 현재 라이브러리에 등록된 모든 증강 가져오기
        List<AugmentSO> allInLibrary = library.GetAllAugments();
        List<AugmentSO> availableList = new List<AugmentSO>();

        Debug.Log($"[Selector] 검사 시작 - 라이브러리 총 개수: {allInLibrary.Count}, 무기 슬롯 여유: {hasWeaponSpace}");

        foreach (var aug in allInLibrary)
        {
            if (aug == null) continue;

            bool canSpawn = IsSpawnableWithLog(aug, equipment, hasGlobalSpace, hasWeaponSpace);
            if (canSpawn)
            {
                availableList.Add(aug);
            }
        }

        Debug.Log($"[Selector] 검사 종료 - 최종 후보: {availableList.Count}개");
        return availableList;
    }

    private static bool IsSpawnableWithLog(AugmentSO aug, PlayerEquipment eq, bool globalSpace, bool weaponSpace)
    {
        string name = string.IsNullOrEmpty(aug.augmentName) ? aug.name : aug.augmentName;

        switch (aug)
        {
            case ItemAugmentSO item:
                ItemAugmentSO existingItem = eq.GetItem(item.augmentName);
                if (existingItem == null) return globalSpace;
                return !existingItem.IsMaxLevel;

            case WeaponAugmentSO wAug:
                // 소지 중인 무기 중에 대상 ID가 있고, 해당 증강이 없거나 최대 레벨이 아닌지 확인
                return eq.WeaponSlots.Any(w => 
                {
                    if (w == null || w.WeaponID != wAug.targetWeaponID) return false;
                    AugmentSO existing = w.GetAugment(wAug.augmentName);
                    if (existing == null) return true; // 아직 없음 -> 등장 가능
                    return !existing.IsMaxLevel;      // 있음 -> 최대 레벨 아니면 등장 가능
                });

            case WeaponUnlockSO unlock:
                if (unlock.weaponPrefab == null)
                {
                    Debug.LogWarning($"[Selector] {name} 탈락: 무기 프리팹이 할당되지 않음!");
                    return false;
                }
                
                string targetID = unlock.weaponPrefab.WeaponID;
                bool alreadyHas = eq.WeaponSlots.Any(w => w != null && w.WeaponID == targetID);
                bool canSpawn = weaponSpace && !alreadyHas;

                // 모든 해금 증강에 대해 상세 로그 출력 (원인 파악용)
                Debug.Log($"[Selector] 무기해금 '{name}' 판정: 슬롯여유({weaponSpace}), 미소지({!alreadyHas}, ID:{targetID}) -> 결과: {canSpawn}");
                return canSpawn;

            default:
                return false;
        }
    }

    public static List<AugmentSO> GetRandomAugments(PlayerEquipment equipment, AugmentLibrarySO library, int count = 3)
    {
        List<AugmentSO> available = GetAvailableAugments(equipment, library);
        return available.OrderBy(x => System.Guid.NewGuid()).Take(count).ToList();
    }
}
