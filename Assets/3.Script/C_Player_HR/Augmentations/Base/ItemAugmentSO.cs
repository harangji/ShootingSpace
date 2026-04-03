using UnityEngine;

/// <summary>
/// 모든 무기와 플레이어 본체에 영향을 주는 전역 아이템의 기반 클래스입니다.
/// </summary>
public abstract class ItemAugmentSO : AugmentSO
{
    [Header("Level Settings")]
    public int level = 1;
    public int maxLevel = 5;

    public bool IsMaxLevel => level >= maxLevel;

    protected virtual void OnEnable()
    {
        type = AugmentType.GlobalItem;
    }

    /// <summary>
    /// 아이템의 레벨을 올립니다.
    /// </summary>
    public void LevelUp()
    {
        level = Mathf.Min(level + 1, maxLevel);
    }

    // 아이템 증강은 발사 로직 수정을 선택적으로 할 수 있게 빈 구현을 제공합니다.
    public override void ModifyFire(FireContext context) { }
}
