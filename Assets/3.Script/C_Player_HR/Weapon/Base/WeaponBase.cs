using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 모든 무기의 기반이 되는 추상 클래스입니다.
/// </summary>
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [TitleGroup("무기 정체성")]
    [LabelText("무기 고유 ID"), Required]
    [SerializeField] protected string weaponID = "Weapon_Default";

    [TitleGroup("증강 시스템")]
    [LabelText("초기 장착 증강")]
    [AssetSelector(Paths = "Assets/Resources/HR_SO/Augments/Weapon_Aug")]
    [SerializeField] protected List<WeaponAugmentSO> initialAugments = new List<WeaponAugmentSO>();

    [ShowInInspector, LabelText("활성화된 증강 리스트"), ReadOnly]
    protected List<IAugment> activeModifiers = new List<IAugment>();

    [TitleGroup("필살기")]
    [LabelText("장착된 필살기"), SerializeReference]
    [Tooltip("이 무기를 장착했을 때 사용할 수 있는 고유 필살기입니다.")]
    public UltimateSkillBase ultimateSkill;

    public string WeaponID => weaponID;

    protected virtual void Awake()
    {
        InitializeAugments();
    }

    protected void InitializeAugments()
    {
        foreach (var augmentSO in initialAugments)
        {
            if (augmentSO == null) continue;
            AddAugment(augmentSO, false);
        }
    }

    public bool AddAugment(AugmentSO augment, bool refresh = true)
    {
        if (augment == null) return false;

        if (augment.type == AugmentType.GlobalItem)
        {
            Debug.LogWarning($"[{weaponID}] 전역 아이템은 무기에 직접 추가할 수 없습니다.");
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

        if (refresh) RequestTotalRefresh();
        return true;
    }

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

    public bool HasAugment(string augmentName) => activeModifiers.Exists(m => (m is AugmentSO so) && so.augmentName == augmentName);

    protected void RequestTotalRefresh()
    {
        PlayerController pc = GetComponentInParent<PlayerController>();
        if (pc != null) pc.RefreshAllStats();
    }

    public abstract void Fire();
    public abstract void RefreshWeaponStats(PlayerContext playerContext, List<ItemAugmentSO> globalItems);
    public abstract string GetDebugStats();
}
