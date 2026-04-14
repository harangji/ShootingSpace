using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 모든 공격 전략의 기반이 되는 스크립터블 오브젝트 클래스입니다.
/// </summary>
public abstract class AttackStrategyBase : ScriptableObject
{
    [Title("전략 정보")]
    [LabelText("전략 설명"), TextArea(2, 5)]
    [Tooltip("이 공격 패턴이 어떤 동작을 하는지 설명합니다.")]
    public string description = "전략에 대한 설명을 입력하세요.";

    /// <summary>
    /// 공격을 실행합니다.
    /// </summary>
    /// <param name="enemy">공격을 수행할 적 본체</param>
    public abstract void Execute(EnemyBase enemy);
}
