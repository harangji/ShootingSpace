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
    [LabelText("스테이지 번호")]
    public int stageNumber;

    [LabelText("등장 적 리스트"), AssetSelector(Paths = "Assets/9.Prefab/HR/Enemy")]
    public List<EnemyBase> spawnableEnemies;

    [LabelText("스폰 간격"), SuffixLabel("초")]
    public float spawnInterval = 1.0f;
    
    [SerializeReference] 
    [LabelText("특수 패턴 리스트")]
    public List<SpecialPatternBase> specialPatterns;
}

/// <summary>
/// 플레이어 주변에 적을 생성하고 관리하는 스포너 클래스입니다.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Title("스테이지 설정")]
    [ListDrawerSettings(ShowIndexLabels = true)]
    [SerializeField] private List<StageConfig> stageConfigs;

    [LabelText("스폰 반경"), SuffixLabel("단위")]
    [SerializeField] private float spawnRadius = 15f;

    [Title("동적 난이도 조절")]
    [LabelText("동적 간격 사용")]
    [SerializeField] private bool useDynamicInterval = true;

    [LabelText("최소 간격 배율"), Range(0.1f, 1.0f)]
    [SerializeField] private float minIntervalMultiplier = 0.5f;

    [LabelText("난이도 최대 도달 시간"), SuffixLabel("초")]
    [SerializeField] private float difficultyRampTime = 300f;

    [Title("참조")]
    [ReadOnly, LabelText("플레이어 트랜스폼")]
    [SerializeField] private Transform playerTransform;

    private StageConfig _currentConfig;
    private float _stageStartTime;
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

    [Button("스테이지 강제 시작", ButtonSizes.Medium), GUIColor(0, 1, 0)]
    public void StartStage(int stageNumber)
    {
        _currentConfig = stageConfigs.Find(c => c.stageNumber == stageNumber);
        if (_currentConfig == null)
        {
            Debug.LogWarning($"[Spawner] 스테이지 {stageNumber}의 설정이 없습니다!");
            return;
        }

        if (EnemyManager.Instance != null && _currentConfig.spawnableEnemies != null)
        {
            foreach (var enemyPrefab in _currentConfig.spawnableEnemies)
            {
                if (enemyPrefab != null)
                    EnemyManager.Instance.PreWarmEnemy(enemyPrefab, 20);
            }
        }

        _isSpawningActive = true;
        _stageStartTime = Time.time;
        _lastSpawnTime = Time.time;

        foreach (var pattern in _currentConfig.specialPatterns)
        {
            if (pattern != null) StartCoroutine(pattern.Execute(this));
        }

        Debug.Log($"[Spawner] 스테이지 {_currentConfig.stageNumber} 스폰 시작!");
    }

    [Button("스폰 중단", ButtonSizes.Medium), GUIColor(1, 0, 0)]
    public void StopSpawning()
    {
        _isSpawningActive = false;
        StopAllCoroutines();
    }

    private void Update()
    {
        if (!_isSpawningActive || _currentConfig == null) return;

        float currentInterval = _currentConfig.spawnInterval;
        if (useDynamicInterval)
        {
            float elapsed = Time.time - _stageStartTime;
            float difficultyRatio = Mathf.Clamp01(elapsed / difficultyRampTime);
            currentInterval *= Mathf.Lerp(1.0f, minIntervalMultiplier, difficultyRatio);
        }

        if (Time.time >= _lastSpawnTime + currentInterval)
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
