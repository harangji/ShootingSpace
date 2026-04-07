using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using Sirenix.OdinInspector;

/// <summary>
/// 각 스테이지의 적 구성과 특수 패턴을 담는 구조체입니다.
/// </summary>
[System.Serializable]
public class StageConfig
{
    public int stageNumber;
    public List<EnemyBase> spawnableEnemies; // 이 단계에서 나올 적들
    public float spawnInterval = 1.0f;       // 이 단계의 기본 스폰 간격
    
    [SerializeReference] 
    public List<SpecialPatternBase> specialPatterns; // 이 단계에서 발생할 특수 패턴들
}

/// <summary>
/// 플레이어 주변에 적을 생성하고 관리하는 스포너 클래스입니다.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Title("Stage Settings")]
    [SerializeField] private List<StageConfig> stageConfigs;
    [SerializeField] private float spawnRadius = 15f; // 소환 거리 (화면 밖)

    [Title("References")]
    [SerializeField] private Transform playerTransform;

    private StageConfig _currentConfig;
    private float _lastSpawnTime;
    private bool _isSpawningActive = false;

    private void Awake()
    {
        InitializeSpawner();
    }

    private void InitializeSpawner()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    /// <summary>
    /// 지정된 스테이지 설정으로 스폰을 시작합니다.
    /// </summary>
    public void StartStage(int stageNumber)
    {
        _currentConfig = stageConfigs.Find(c => c.stageNumber == stageNumber);
        if (_currentConfig == null)
        {
            Debug.LogWarning($"[Spawner] 스테이지 {stageNumber}의 설정이 없습니다!");
            return;
        }

        // --- 스테이지 시작 시 적 프리웜(Pre-warm) 수행 ---
        if (EnemyManager.Instance != null && _currentConfig.spawnableEnemies != null)
        {
            foreach (var enemyPrefab in _currentConfig.spawnableEnemies)
            {
                if (enemyPrefab != null)
                {
                    EnemyManager.Instance.PreWarmEnemy(enemyPrefab, 20); // 각 종류별 20마리씩 미리 생성
                }
            }
        }

        _isSpawningActive = true;
        _lastSpawnTime = Time.time;

        // 특수 패턴들 실행 (코루틴)
        foreach (var pattern in _currentConfig.specialPatterns)
        {
            if (pattern != null) StartCoroutine(pattern.Execute(this));
        }

        Debug.Log($"[Spawner] 스테이지 {_currentConfig.stageNumber} 스폰 시작!");
    }

    public void StopSpawning()
    {
        _isSpawningActive = false;
        StopAllCoroutines(); // 진행 중인 특수 패턴 중단
    }

    private void Update()
    {
        if (!_isSpawningActive || _currentConfig == null) return;

        if (Time.time >= _lastSpawnTime + _currentConfig.spawnInterval)
        {
            SpawnRandomEnemy();
            _lastSpawnTime = Time.time;
        }
    }

    private void SpawnRandomEnemy()
    {
        if (_currentConfig.spawnableEnemies == null || _currentConfig.spawnableEnemies.Count == 0) return;

        EnemyBase prefab = _currentConfig.spawnableEnemies[Random.Range(0, _currentConfig.spawnableEnemies.Count)];
        if (prefab == null) return;

        Vector2 spawnPos = GetRandomSpawnPosition();
        SpawnEnemyAt(prefab, spawnPos);
    }

    public Vector2 GetRandomSpawnPosition()
    {
        if (playerTransform == null) return Vector2.zero;
        return (Vector2)playerTransform.position + Random.insideUnitCircle.normalized * spawnRadius;
    }

    public GameObject SpawnEnemyAt(EnemyBase prefab, Vector2 position)
    {
        if (EnemyManager.Instance == null) return null;

        GameObject enemyObj = EnemyManager.Instance.GetEnemy(prefab);
        enemyObj.transform.position = position;
        return enemyObj;
    }
}

