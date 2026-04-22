using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 모든 무기와 플레이어 본체에 영향을 주는 전역 아이템의 기반 클래스입니다.
/// </summary>
public abstract class ItemAugmentSO : AugmentSO
{
    protected virtual void OnEnable()
    {
        type = AugmentType.GlobalItem;
    }

    public override void ModifyFire(FireContext context) { }
}
