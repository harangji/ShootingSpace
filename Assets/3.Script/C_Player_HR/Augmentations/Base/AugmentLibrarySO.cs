using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 모든 증강 데이터를 한곳에서 관리하는 중앙 보관소입니다.
/// </summary>
[CreateAssetMenu(fileName = "AugmentLibrary", menuName = "ShootingSpace/Augments/Library")]
[Searchable]
public class AugmentLibrarySO : SerializedScriptableObject
{
    [Title("Library Settings")]
    [FolderPath]
    [SerializeField] private string rootFolder = "Assets/Resources/HR_SO/Augments";

    [TabGroup("Main", "전역 증강 (Global)", SdfIconType.Globe)]
    public List<ItemAugmentSO> globalItems = new List<ItemAugmentSO>();

    [TabGroup("Main", "무기 증강 (Weapon)", SdfIconType.Bullseye)]
    [ShowInInspector]
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
    public Dictionary<string, List<WeaponAugmentSO>> weaponAugmentGroups = new Dictionary<string, List<WeaponAugmentSO>>();

    [TabGroup("Main", "무기 해금 (Unlock)", SdfIconType.Key)]
    public List<WeaponUnlockSO> weaponUnlocks = new List<WeaponUnlockSO>();

    [Button(ButtonSizes.Large, Name = "라이브러리 전체 갱신 (폴더 기반)")]
    [GUIColor(0.4f, 1f, 0.4f)]
    public void RefreshLibrary()
    {
#if UNITY_EDITOR
        globalItems.Clear();
        weaponAugmentGroups.Clear();
        weaponUnlocks.Clear();

        ScanFolder<ItemAugmentSO>(rootFolder + "/Global_Aug", globalItems, true);
        ScanFolder<WeaponUnlockSO>(rootFolder + "/Weapon_Unlock", weaponUnlocks, false);
        ScanWeaponAugments(rootFolder + "/Weapon_Aug");

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Library] 갱신 완료! 총 {GetAllAugments().Count}개의 증강이 등록되었습니다.");
#endif
    }

    private void ScanWeaponAugments(string folderPath)
    {
#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder(folderPath)) return;
        string[] guids = AssetDatabase.FindAssets("t:WeaponAugmentSO", new[] { folderPath });
        foreach (string guid in guids)
        {
            WeaponAugmentSO augment = AssetDatabase.LoadAssetAtPath<WeaponAugmentSO>(AssetDatabase.GUIDToAssetPath(guid));
            if (augment == null) continue;

            // 이름이 비어있으면 에셋 이름으로 채우기
            if (string.IsNullOrEmpty(augment.augmentName)) augment.augmentName = augment.name;

            string weaponID = string.IsNullOrEmpty(augment.targetWeaponID) ? "Unknown_Weapon" : augment.targetWeaponID;
            if (!weaponAugmentGroups.ContainsKey(weaponID)) weaponAugmentGroups[weaponID] = new List<WeaponAugmentSO>();
            if (!weaponAugmentGroups[weaponID].Contains(augment)) weaponAugmentGroups[weaponID].Add(augment);
            EditorUtility.SetDirty(augment);
        }
#endif
    }

    private void ScanFolder<T>(string folderPath, List<T> targetList, bool isGlobal) where T : AugmentSO
    {
#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder(folderPath)) return;
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folderPath });
        foreach (string guid in guids)
        {
            T augment = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
            if (augment == null) continue;

            // 이름 보정
            if (string.IsNullOrEmpty(augment.augmentName)) augment.augmentName = augment.name;

            if (isGlobal && augment is ItemAugmentSO item && !item.augmentName.StartsWith("[G] "))
            {
                item.augmentName = "[G] " + item.augmentName;
            }

            if (!targetList.Contains(augment)) targetList.Add(augment);
            EditorUtility.SetDirty(augment);
        }
#endif
    }

    public List<AugmentSO> GetAllAugments()
    {
        var all = new List<AugmentSO>();
        if (globalItems != null) all.AddRange(globalItems.Cast<AugmentSO>());
        if (weaponAugmentGroups != null)
        {
            foreach (var group in weaponAugmentGroups.Values) all.AddRange(group.Cast<AugmentSO>());
        }
        if (weaponUnlocks != null) all.AddRange(weaponUnlocks.Cast<AugmentSO>());
        return all;
    }

    public List<WeaponAugmentSO> GetAugmentsForWeapon(string weaponID)
    {
        if (weaponAugmentGroups != null && weaponAugmentGroups.TryGetValue(weaponID, out var list)) return list;
        return new List<WeaponAugmentSO>();
    }
}
