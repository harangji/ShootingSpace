using UnityEngine;

/// <summary>
/// 특정 무기에만 장착될 수 있는 무기 전용 증강의 기반 클래스입니다.
/// </summary>
public abstract class WeaponAugmentSO : AugmentSO
{
    protected virtual void OnEnable()
    {
        type = AugmentType.WeaponUnique;
    }

    // 무기 전용 증강은 발사 로직 수정을 필수로 구현해야 합니다.
}
