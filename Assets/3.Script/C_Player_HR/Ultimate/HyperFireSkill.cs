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
        // 1. GameManager를 통해 플레이어 스탯 매니저에 접근
        var stats = GameManager.Instance.playerStats;
        if (stats != null)
        {
            stats.SetUltimateMultipliers(damageMultiplier, fireRateMultiplier);
        }

        // 2. 모든 무기 스탯 갱신
        controller.RefreshAllStats();
        
        Debug.Log($"[Ultimate] {SkillName} 발동! 공격력/연사속도 {fireRateMultiplier}배!!");
    }

    public override void Deactivate(PlayerController controller)
    {
        // 1. 배율 원복 (1.0으로 초기화)
        var stats = GameManager.Instance.playerStats;
        if (stats != null)
        {
            stats.SetUltimateMultipliers(1.0f, 1.0f);
        }

        // 2. 모든 무기 스탯 갱신
        controller.RefreshAllStats();
        
        Debug.Log($"[Ultimate] {SkillName} 종료. 스탯 원복.");
    }
}
