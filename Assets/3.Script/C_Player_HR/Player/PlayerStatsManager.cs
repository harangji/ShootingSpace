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

    private float _ultimateDamageMultiplier = 1.0f;
    private float _ultimateFireRateMultiplier = 1.0f;

    /// <summary>
    /// 필살기 전용 배율을 설정합니다.
    /// </summary>
    public void SetUltimateMultipliers(float damage, float fireRate)
    {
        _ultimateDamageMultiplier = damage;
        _ultimateFireRateMultiplier = fireRate;
    }

    /// <summary>
    /// 아이템 증강 리스트를 바탕으로 최종 플레이어 스탯을 갱신합니다.
    /// </summary>
    public void Refresh(List<ItemAugmentSO> items)
    {
        CurrentContext = new PlayerContext();

        // 1. 아이템 효과 적용
        foreach (var item in items)
        {
            if (item != null) item.ModifyPlayer(CurrentContext);
        }

        // 2. 필살기 배율 적용
        CurrentContext.ultimateDamageMultiplier = _ultimateDamageMultiplier;
        CurrentContext.ultimateFireRateMultiplier = _ultimateFireRateMultiplier;

        MaxHealth = Mathf.RoundToInt(baseMaxHealth * CurrentContext.maxHealthMultiplier);
        MoveSpeed = baseMoveSpeed * CurrentContext.moveSpeedMultiplier;
        ExpMultiplier = CurrentContext.expGainMultiplier;
    }
}
