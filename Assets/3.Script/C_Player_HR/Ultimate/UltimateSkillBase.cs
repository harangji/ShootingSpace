using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 필살기의 공통 데이터를 관리하는 추상 베이스 클래스입니다.
/// </summary>
[System.Serializable]
public abstract class UltimateSkillBase : IUltimateSkill
{
    [Title("필살기 기본 정보")]
    [LabelText("스킬 이름")]
    [SerializeField] private string skillName = "New Ultimate";

    [LabelText("소모 게이지"), SuffixLabel("Point")]
    [SerializeField] private float requiredGauge = 100f;

    [LabelText("지속 시간"), SuffixLabel("초")]
    [SerializeField] private float duration = 5f;

    public string SkillName => skillName;
    public float RequiredGauge => requiredGauge;
    public float Duration => duration;

    public abstract void Activate(PlayerController controller);
    public abstract void Deactivate(PlayerController controller);
}
