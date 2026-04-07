using UnityEngine;
using System.Collections;

/// <summary>
/// 대량의 몬스터가 플레이어를 빠르게 지나쳐 가는 특수 패턴입니다.
/// </summary>
[System.Serializable]
public class SwarmPattern : SpecialPatternBase
{
    [Header("Swarm Settings")]
    public EnemyBase enemyPrefab;
    public int spawnCount = 10;
    public float spawnInterval = 0.2f; // 각 몬스터 사이의 간격
    public float swarmMoveSpeedMultiplier = 2.0f; // 떼로 나올 때의 추가 속도

    public override IEnumerator Execute(EnemySpawner spawner)
    {
        if (enemyPrefab == null) yield break;

        yield return new WaitForSeconds(delayBeforeStart);

        Debug.Log($"[Pattern] {patternName} 발동! 몬스터 {spawnCount}마리 소환!");

        // 화면 밖 랜덤한 지점에서 일직선으로 지나가도록 설정
        Vector2 startPos = spawner.GetRandomSpawnPosition();
        Vector2 targetPos = -startPos; // 반대 방향으로 질주

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject enemyObj = spawner.SpawnEnemyAt(enemyPrefab, startPos);
            
            if (enemyObj.TryGetComponent<EnemyBase>(out var enemy))
            {
                // 특수 패턴용 이동 로직을 위해 속도만 일단 빠르게 설정
                // 실제로는 특수 이동 로직(예: MoveStraight)을 주입할 수도 있습니다.
                // 여기서는 단순히 해당 객체를 목표 방향으로 쏘아보내는 임시 처리를 합니다.
                enemy.transform.up = (targetPos - startPos).normalized;
                
                // 만약 EnemyBase에 속도 변조 기능이 있다면 여기서 수정 가능!
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
