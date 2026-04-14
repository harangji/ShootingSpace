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

    [LabelText("설명"), TextArea(3, 5)]
    public string description;

    [LabelText("증강 타입")]
    public AugmentType type;
    
    [LabelText("대상 무기 ID"), Tooltip("무기 고유 증강일 경우 대상 무기의 ID와 일치해야 합니다.")]
    [ShowIf("type", AugmentType.WeaponUnique)]
    public string targetWeaponID;

    public virtual void ModifyWeapon(WeaponContext context) { }
    public abstract void ModifyFire(FireContext context);
    public virtual void ModifyPlayer(PlayerContext context) { }
}
