using UnityEngine;

public enum AugmentType
{
    WeaponUnique, // 특정 무기 전용 증강
    GlobalItem    // 모든 무기에 영향을 주는 아이템 증강
}

/// <summary>
/// 인스펙터에서 할당 가능한 증강 에셋의 베이스 클래스입니다.
/// </summary>
public abstract class AugmentSO : ScriptableObject, IAugment
{
    [Header("Augment Info")]
    public string augmentName;
    [TextArea] public string description;
    public AugmentType type;
    
    [Tooltip("무기 고유 증강일 경우, 대상 무기의 ID와 일치해야 합니다.")]
    public string targetWeaponID;

    // 무기 스탯 수정 (필요한 경우 오버라이드)
    public virtual void ModifyWeapon(WeaponContext context) { }

    // 발사 데이터 수정 (필요한 경우 오버라이드)
    public abstract void ModifyFire(FireContext context);
}
