using UnityEngine;
using System.Collections;

/// <summary>
/// 스테이지 도중 발생하는 특수 패턴(이벤트)의 기반 클래스입니다.
/// </summary>
[System.Serializable]
public abstract class SpecialPatternBase
{
    public string patternName = "New Pattern";
    public float delayBeforeStart = 0f; // 패턴 시작 전 대기 시간

    /// <summary>
    /// 패턴을 실행합니다.
    /// </summary>
    /// <param name="spawner">스포너 참조</param>
    public abstract IEnumerator Execute(EnemySpawner spawner);
}
