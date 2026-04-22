using UnityEngine;
using Sirenix.OdinInspector;
using System.Threading.Tasks;

/// <summary>
/// 모든 공격 전략의 기반이 되는 모노베헤이비어 클래스입니다.
/// </summary>
public abstract class AttackStrategyBase : MonoBehaviour
{
    [Title("전략 정보")]
    [LabelText("전략 설명"), TextArea(2, 5)]
    [Tooltip("이 공격 패턴이 어떤 동작을 하는지 설명합니다.")]
    public string description = "전략에 대한 설명을 입력하세요.";

    [LabelText("공격 사거리")]
    [Tooltip("이 공격 전략이 발동되는 사거리입니다.")]
    public float attackRange = 5f;

    [LabelText("기본 데미지")]
    [Tooltip("이 공격 전략의 기본 데미지 값입니다.")]
    public int damage = 10;
    
    public abstract Task ExecuteAsync(EnemyBase enemy);
    
    public abstract bool CanExecute();
}
