using UnityEngine;
using UnityEngine.Pool;
using System;
using System.Collections.Generic;

/// <summary>
/// 모든 적 개체의 등록, 해제, 이벤트 및 풀링을 관리하는 중앙 매니저입니다.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public event Action<EnemyBase> OnEnemyKilled;
    
    [Header("Settings")]
    [SerializeField] private Transform enemyContainer;

    // 현재 필드에 활성화된 모든 적 리스트
    private List<EnemyBase> _activeEnemies = new List<EnemyBase>();
    public IReadOnlyList<EnemyBase> ActiveEnemies => _activeEnemies;

    // 프리팹별 풀 관리 딕셔너리
    private Dictionary<string, IObjectPool<GameObject>> _pools = new Dictionary<string, IObjectPool<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (enemyContainer == null)
        {
            GameObject obj = new GameObject("EnemyContainer");
            enemyContainer = obj.transform;
        }
    }

    /// <summary>
    /// 적 프리팹으로부터 풀링된 객체를 가져옵니다.
    /// </summary>
    public GameObject GetEnemy(EnemyBase prefab)
    {
        string key = prefab.name;
        if (!_pools.ContainsKey(key))
        {
            // 처음 요청 시 기본적으로 20마리 프리웜
            _pools[key] = CreatePool(prefab, 20);
        }

        return _pools[key].Get();
    }

    /// <summary>
    /// 특정 적 프리팹을 미리 일정량 생성해둡니다.
    /// </summary>
    public void PreWarmEnemy(EnemyBase prefab, int count)
    {
        string key = prefab.name;
        if (!_pools.ContainsKey(key))
        {
            _pools[key] = CreatePool(prefab, count);
        }
    }

    private IObjectPool<GameObject> CreatePool(EnemyBase prefab, int preWarmCount)
    {
        IObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            () => CreateEnemyInstance(prefab),
            OnGetEnemy,
            OnReleaseEnemy,
            OnDestroyEnemy,
            true, 20, 100
        );

        if (preWarmCount > 0)
        {
            GameObject[] tempArray = new GameObject[preWarmCount];
            for (int i = 0; i < preWarmCount; i++)
            {
                tempArray[i] = pool.Get();
            }
            for (int i = 0; i < preWarmCount; i++)
            {
                pool.Release(tempArray[i]);
            }
        }

        return pool;
    }

    /// <summary>
    /// 적을 풀로 반납합니다.
    /// </summary>
    public void ReleaseEnemy(EnemyBase enemy)
    {
        string key = enemy.gameObject.name; // 생성 시 이름을 프리팹 이름으로 고정함
        if (_pools.TryGetValue(key, out var pool))
        {
            pool.Release(enemy.gameObject);
        }
        else
        {
            Destroy(enemy.gameObject);
        }
    }

    private GameObject CreateEnemyInstance(EnemyBase prefab)
    {
        GameObject obj = Instantiate(prefab.gameObject, enemyContainer);
        obj.name = prefab.name; // 풀링 키로 사용하기 위해 이름을 고정
        return obj;
    }

    private void OnGetEnemy(GameObject obj) => obj.SetActive(true);
    private void OnReleaseEnemy(GameObject obj) => obj.SetActive(false);
    private void OnDestroyEnemy(GameObject obj) => Destroy(obj);

    public void RegisterEnemy(EnemyBase enemy)
    {
        if (!_activeEnemies.Contains(enemy))
            _activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyBase enemy)
    {
        _activeEnemies.Remove(enemy);
    }

    public void ReportEnemyKilled(EnemyBase enemy)
    {
        OnEnemyKilled?.Invoke(enemy);
        // Unregister는 OnDisable에서 자동으로 처리
    }

    public void KillAllEnemies()
    {
        List<EnemyBase> enemiesToKill = new List<EnemyBase>(_activeEnemies);
        foreach (var enemy in enemiesToKill)
        {
            enemy.TakeDamage(9999);
        }
    }
}
