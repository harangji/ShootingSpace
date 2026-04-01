using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 플레이어의 기본 스탯과 증강에 의한 최종 스탯을 관리하는 클래스입니다.
/// </summary>
public class PlayerStatsManager : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private int baseMaxHealth = 100;
    [SerializeField] private float baseMoveSpeed = 5f;

    public int MaxHealth { get; private set; }
    public float MoveSpeed { get; private set; }
    public float ExpMultiplier { get; private set; }
    public PlayerContext CurrentContext { get; private set; } = new PlayerContext();

    /// <summary>
    /// 아이템 증강 리스트를 바탕으로 최종 플레이어 스탯을 갱신합니다.
    /// </summary>
    public void Refresh(List<ItemAugmentSO> items)
    {
        CurrentContext = new PlayerContext();

        foreach (var item in items)
        {
            if (item != null) item.ModifyPlayer(CurrentContext);
        }

        MaxHealth = Mathf.RoundToInt(baseMaxHealth * CurrentContext.maxHealthMultiplier);
        MoveSpeed = baseMoveSpeed * CurrentContext.moveSpeedMultiplier;
        ExpMultiplier = CurrentContext.expGainMultiplier;
    }
}
