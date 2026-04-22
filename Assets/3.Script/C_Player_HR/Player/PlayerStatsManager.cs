using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

/// <summary>
/// 플레이어의 기본 스탯과 증강에 의한 최종 스탯을 관리하는 클래스입니다.
/// </summary>
public class PlayerStatsManager : MonoBehaviour
{
    [Title("기본 능력치")]
    [LabelText("기본 최대 체력")]
    [SerializeField] private int baseMaxHealth = 100;

    [LabelText("기본 이동 속도")]
    [SerializeField] private float baseMoveSpeed = 5f;

    [Title("최종 능력치 (증강 적용됨)")]
    [ReadOnly, LabelText("최종 최대 체력"), GUIColor(1, 0.8f, 0.8f)]
    [ShowInInspector] public int MaxHealth { get; private set; }

    [ReadOnly, LabelText("현재 체력"), GUIColor(1, 0.5f, 0.5f)]
    [ShowInInspector] public ClampedInt Health { get; private set; }

    [ReadOnly, LabelText("최종 이동 속도"), GUIColor(0.8f, 1, 0.8f)]
    [ShowInInspector] public float MoveSpeed { get; private set; }

    [ReadOnly, LabelText("경험치 획득 배율")]
    [ShowInInspector] public float ExpMultiplier { get; private set; }

    public PlayerContext CurrentContext { get; private set; } = new PlayerContext();

    private float _ultimateDamageMultiplier = 1.0f;
    private float _ultimateFireRateMultiplier = 1.0f;

    public event System.Action OnDamaged;

    public void TakeDamage(int damage)
    {
        Health.Decrease(damage);
        OnDamaged?.Invoke();
    }

    public void Heal(int amount) => Health.Increase(amount);

    public void SetUltimateMultipliers(float damage, float fireRate)
    {
        _ultimateDamageMultiplier = damage;
        _ultimateFireRateMultiplier = fireRate;
    }

    public void Refresh(List<ItemAugmentSO> items)
    {
        CurrentContext = new PlayerContext();

        foreach (var item in items)
        {
            if (item != null) item.ModifyPlayer(CurrentContext);
        }

        CurrentContext.ultimateDamageMultiplier = _ultimateDamageMultiplier;
        CurrentContext.ultimateFireRateMultiplier = _ultimateFireRateMultiplier;

        int oldMaxHealth = MaxHealth;
        MaxHealth = Mathf.RoundToInt(baseMaxHealth * CurrentContext.maxHealthMultiplier);
        
        if (Health == null)
        {
            Health = new ClampedInt(0, MaxHealth, MaxHealth);
        }
        else
        {
            int currentHealth = Health.Current;
            Health.SetMinMax(0, MaxHealth);
            // 필요 시 추가적인 체력 보정 로직 (예: 최대 체력이 늘어나면 그대로 유지)
        }

        MoveSpeed = baseMoveSpeed * CurrentContext.moveSpeedMultiplier;
        ExpMultiplier = CurrentContext.expGainMultiplier;
    }
}
