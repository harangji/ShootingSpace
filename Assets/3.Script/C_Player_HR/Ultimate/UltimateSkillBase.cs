using UnityEngine;

/// <summary>
/// 모든 필살기가 구현해야 하는 인터페이스입니다.
/// </summary>
public interface IUltimateSkill
{
    string SkillName { get; }
    float RequiredGauge { get; }
    float Duration { get; }
    
    void Activate(PlayerController controller);
    void Deactivate(PlayerController controller);
}

/// <summary>
/// 필살기의 공통 데이터를 관리하는 추상 베이스 클래스입니다.
/// [System.Serializable]을 통해 인스펙터에서 수정 가능하게 합니다.
/// </summary>
[System.Serializable]
public abstract class UltimateSkillBase : IUltimateSkill
{
    [Header("Ultimate Info")]
    [SerializeField] private string skillName = "New Ultimate";
    [SerializeField] private float requiredGauge = 100f;
    [SerializeField] private float duration = 5f;

    public string SkillName => skillName;
    public float RequiredGauge => requiredGauge;
    public float Duration => duration;

    public abstract void Activate(PlayerController controller);
    public abstract void Deactivate(PlayerController controller);
}
