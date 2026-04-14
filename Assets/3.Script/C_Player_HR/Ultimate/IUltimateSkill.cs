/// <summary>
/// 모든 필살기가 구현해야 하는 기본 인터페이스입니다.
/// </summary>
public interface IUltimateSkill
{
    string SkillName { get; }
    float RequiredGauge { get; }
    float Duration { get; }
    
    void Activate(PlayerController controller);
    void Deactivate(PlayerController controller);
}
