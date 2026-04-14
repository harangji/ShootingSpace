using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 모든 무기와 플레이어 본체에 영향을 주는 전역 아이템의 기반 클래스입니다.
/// </summary>
public abstract class ItemAugmentSO : AugmentSO
{
    [Title("레벨 설정")]
    [LabelText("현재 레벨"), Range(1, 10)]
    public int level = 1;

    [LabelText("최대 레벨"), Range(1, 10)]
    public int maxLevel = 5;

    [ShowInInspector, LabelText("최대 레벨 도달 여부"), ReadOnly]
    public bool IsMaxLevel => level >= maxLevel;

    protected virtual void OnEnable()
    {
        type = AugmentType.GlobalItem;
    }

    [Button("레벨 업 Test", ButtonSizes.Medium)]
    public void LevelUp()
    {
        level = Mathf.Min(level + 1, maxLevel);
    }

    public override void ModifyFire(FireContext context) { }
}
