using UnityEngine;
using UnityEngine.Pool;
using System;

/// <summary>
/// 풀링되는 오브젝트가 구현해야 하는 인터페이스입니다.
/// </summary>
/// <typeparam name="T">오브젝트의 컴포넌트 타입</typeparam>
public interface IPoolable<T> where T : Component
{
    void SetPool(IObjectPool<T> pool);
}

/// <summary>
/// 다양한 타입의 오브젝트를 관리할 수 있는 범용 오브젝트 풀링 클래스입니다.
/// </summary>
/// <typeparam name="T">풀링할 오브젝트의 타입 (Component 상속)</typeparam>
public class GenericObjectPool<T> where T : Component
{
    private IObjectPool<T> _pool;
    private T _prefab;
    private Transform _parent;
    private Transform _subParent; // 클래스별 전용 부모 추가!

    public GenericObjectPool(T prefab, Transform parent, int defaultCapacity = 20, int maxSize = 200, int preWarmCount = 0)
    {
        _prefab = prefab;
        _parent = parent;

        // 클래스 이름을 가진 하위 부모 생성 (예: Bullet_01, Bullet_02)
        GameObject subParentObj = new GameObject($"{typeof(T).Name}_Pool");
        _subParent = subParentObj.transform;
        
        if (_parent != null)
        {
            _subParent.SetParent(_parent);
        }

        // 유니티 내장 ObjectPool 생성
        _pool = new ObjectPool<T>(
            CreateInstance,
            OnGet,
            OnRelease,
            OnDestroyInstance,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        // --- 프리웜(Pre-warm): 초기 개수만큼 미리 생성하여 풀에 채워둠 ---
        if (preWarmCount > 0)
        {
            // preWarmCount는 maxSize를 넘지 않도록 제한
            int count = Mathf.Min(preWarmCount, maxSize);
            T[] tempArray = new T[count];

            // Get()을 호출하여 인스턴스를 생성 및 활성화
            for (int i = 0; i < count; i++)
            {
                tempArray[i] = _pool.Get();
            }

            // 생성된 인스턴스들을 다시 Release()하여 비활성화 상태로 풀에 반납
            for (int i = 0; i < count; i++)
            {
                _pool.Release(tempArray[i]);
            }
        }
    }

    /// <summary>
    /// 풀에서 오브젝트를 가져옵니다.
    /// </summary>
    public T Get() => _pool.Get();

    /// <summary>
    /// 오브젝트를 풀로 반납합니다.
    /// </summary>
    public void Release(T element) => _pool.Release(element);

    // --- 풀링 이벤트 핸들러 ---

    private T CreateInstance()
    {
        // 이제 전용 부모(_subParent) 밑에 생성됩니다!
        T instance = UnityEngine.Object.Instantiate(_prefab, _subParent);
        
        // 만약 오브젝트가 IPoolable을 구현하고 있다면, 소속 풀을 알려줍니다.
        if (instance is IPoolable<T> poolable)
        {
            poolable.SetPool(_pool);
        }

        return instance;
    }

    private void OnGet(T instance)
    {
        instance.gameObject.SetActive(true);
    }

    private void OnRelease(T instance)
    {
        instance.gameObject.SetActive(false);
    }

    private void OnDestroyInstance(T instance)
    {
        UnityEngine.Object.Destroy(instance.gameObject);
    }
}
