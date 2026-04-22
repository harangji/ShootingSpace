using UnityEngine;
using Sirenix.OdinInspector;

public enum AugmentType
{
    [LabelText("무기 고유 증강")] WeaponUnique,
    [LabelText("전역 아이템 증강")] GlobalItem,
    [LabelText("무기 해금")] NewWeapon
}

/// <summary>
/// 인스펙터에서 할당 가능한 증강 에셋의 베이스 클래스입니다.
/// </summary>
public abstract class AugmentSO : ScriptableObject, IAugment
{
    [Title("증강 기본 정보", "증강의 정체성을 정의합니다.")]
    [LabelText("증강 이름"), Tooltip("게임 내에서 표시될 이름입니다.")]
    public string augmentName;

    // 리스트나 하이러키 표시용 이름
    public string DisplayName => $"{augmentName} (Lv.{level})";

    [LabelText("기본 설명"), TextArea(3, 5)]
    public string description;

    [PropertyOrder(10)]
    [Title("실시간 UI 프리뷰", "게임 내 UI에 표시될 최종 형태입니다.")]
    [ShowInInspector, HideLabel, ReadOnly, TextArea(5, 8), GUIColor(0.8f, 1f, 0.8f)]
    public string UIPreview => GetDescription();

    /// <summary>
    /// 현재 수치를 포함한 동적인 설명을 반환합니다.
    /// </summary>
    public virtual string GetDescription()
    {
        return description;
    }

    [LabelText("증강 타입")]
    public AugmentType type;
    
    [LabelText("대상 무기 ID"), Tooltip("무기 고유 증강일 경우 대상 무기의 ID와 일치해야 합니다.")]
    [ShowIf("type", AugmentType.WeaponUnique)]
    public string targetWeaponID;

    [Title("레벨 설정")]
    [LabelText("현재 레벨"), Range(1, 10)]
    public int level = 1;

    [LabelText("최대 레벨"), Range(1, 10)]
    public int maxLevel = 5;

    [ShowInInspector, LabelText("최대 레벨 도달 여부"), ReadOnly]
    public bool IsMaxLevel => level >= maxLevel;

    [Button("레벨 업 Test", ButtonSizes.Medium)]
    public virtual void LevelUp()
    {
        level = Mathf.Min(level + 1, maxLevel);
    }

    public virtual void ModifyWeapon(WeaponContext context) { }
    public abstract void ModifyFire(FireContext context);
    public virtual void ModifyPlayer(PlayerContext context) { }
}
