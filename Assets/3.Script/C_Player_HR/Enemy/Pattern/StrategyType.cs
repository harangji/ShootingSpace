using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 몬스터가 사용할 수 있는 공격 전략의 종류를 정의하는 열거형입니다.
/// </summary>
public enum StrategyType
{
    [LabelText("일반 사격"), Tooltip("플레이어를 향해 기본 탄환을 발사하는 가장 기초적인 공격입니다.")]
    Shoot,

    [LabelText("돌진 공격"), Tooltip("공격 경로를 인디케이터로 예고한 뒤, 플레이어에게 빠르게 돌격하여 피해를 줍니다.")]
    Dash,

    [LabelText("레이저 발사"), Tooltip("일정 시간 동안 충전 후 직선상의 모든 적을 관통하는 강력한 레이저를 발사합니다.")]
    Laser,

    [LabelText("근접 자폭"), Tooltip("플레이어에게 매우 근접했을 때 즉시 자폭하여 좁은 범위에 폭발 피해를 줍니다.")]
    SelfDestruct,

    [LabelText("자폭 분열"), Tooltip("자폭하면서 주변 8방향으로 추가 투사체를 뿌려 최후의 저항을 합니다.")]
    ExplodeSplit
}
