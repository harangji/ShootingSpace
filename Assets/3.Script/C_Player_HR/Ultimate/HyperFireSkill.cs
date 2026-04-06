using UnityEngine;

/// <summary>
/// 일정 시간 동안 모든 무기의 연사 속도와 공격력을 대폭 강화하는 필살기 클래스입니다.
/// </summary>
[System.Serializable]
public class HyperFireSkill : UltimateSkillBase
{
    [Header("Hyper Fire Buff Values")]
    public float fireRateMultiplier = 3.0f;
    public float damageMultiplier = 3.0f;

    public override void Activate(PlayerController controller)
    {
        // 1. 플레이어 컨텍스트에 필살기 배율 적용
        PlayerStatsManager stats = controller.GetComponent<PlayerStatsManager>();
        if (stats != null)
        {
            stats.CurrentContext.ultimateFireRateMultiplier = fireRateMultiplier;
            stats.CurrentContext.ultimateDamageMultiplier = damageMultiplier;
        }

        // 2. 모든 무기 스탯 갱신
        controller.RefreshAllStats();
        
        Debug.Log($"[Ultimate] {SkillName} 발동! 공격력/연사속도 {fireRateMultiplier}배!!");
    }

    public override void Deactivate(PlayerController controller)
    {
        // 1. 배율 원복
        PlayerStatsManager stats = controller.GetComponent<PlayerStatsManager>();
        if (stats != null)
        {
            stats.CurrentContext.ultimateFireRateMultiplier = 1.0f;
            stats.CurrentContext.ultimateDamageMultiplier = 1.0f;
        }

        // 2. 모든 무기 스탯 갱신
        controller.RefreshAllStats();
        
        Debug.Log($"[Ultimate] {SkillName} 종료. 스탯 원복.");
    }
}
