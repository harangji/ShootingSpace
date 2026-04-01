using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 무기의 기반이 되는 추상 클래스입니다.
/// 증강 관리 및 기본 스탯 갱신 인터페이스를 제공합니다.
/// </summary>
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [Header("Weapon Identity")]
    [SerializeField] protected string weaponID = "Weapon_Default";

    [Header("Augmentations")]
    [SerializeField] protected List<WeaponAugmentSO> initialAugments = new List<WeaponAugmentSO>();
    protected List<IAugment> activeModifiers = new List<IAugment>();

    public string WeaponID => weaponID;

    protected virtual void Awake()
    {
        InitializeAugments();
    }

    /// <summary>
    /// 초기 증강들을 설정합니다.
    /// </summary>
    protected void InitializeAugments()
    {
        foreach (var augmentSO in initialAugments)
        {
            if (augmentSO == null) continue;
            AddAugment(augmentSO, false);
        }
    }

    /// <summary>
    /// 새로운 고유 증강을 추가합니다.
    /// </summary>
    public bool AddAugment(AugmentSO augment, bool refresh = true)
    {
        if (augment == null) return false;

        // 분리된 시스템에 따라 GlobalItem은 무기에 직접 추가할 수 없습니다.
        if (augment.type == AugmentType.GlobalItem)
        {
            Debug.LogWarning($"[{weaponID}] 전역 아이템은 무기에 직접 추가할 수 없습니다. PlayerController를 사용하십시오.");
            return false;
        }

        if (augment.type == AugmentType.WeaponUnique && augment.targetWeaponID != weaponID)
        {
            Debug.LogWarning($"[{weaponID}] ID 불일치 증강입니다.");
            return false;
        }

        if (HasAugment(augment.augmentName)) return false;

        AugmentSO instance = Instantiate(augment);
        activeModifiers.Add(instance);

        if (refresh)
        {
            RequestTotalRefresh();
        }
        return true;
    }

    /// <summary>
    /// 증강을 제거합니다.
    /// </summary>
    public virtual bool RemoveAugment(string augmentName)
    {
        IAugment target = activeModifiers.Find(m => (m is AugmentSO so) && so.augmentName == augmentName);
        if (target != null)
        {
            activeModifiers.Remove(target);
            if (target is Object obj) Destroy(obj);
            RequestTotalRefresh();
            return true;
        }
        return false;
    }

    protected bool HasAugment(string augmentName)
    {
        return activeModifiers.Exists(m => (m is AugmentSO so) && so.augmentName == augmentName);
    }

    /// <summary>
    /// 플레이어에게 전체 스탯 갱신을 요청합니다.
    /// </summary>
    protected void RequestTotalRefresh()
    {
        PlayerController pc = GetComponentInParent<PlayerController>();
        if (pc != null) pc.RefreshAllStats();
    }

    // --- 추상 메서드: 자식 클래스에서 구현해야 합니다. ---
    public abstract void Fire();
    public abstract void RefreshWeaponStats(PlayerContext playerContext, List<ItemAugmentSO> globalItems);
    public abstract string GetDebugStats();
}
