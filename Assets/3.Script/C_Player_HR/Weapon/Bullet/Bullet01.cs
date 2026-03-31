using UnityEngine;
using UnityEngine.Pool; // 유니티 내장 풀링 시스템 라이브러리 추가!
using ShootingSpace.Core;

/// <summary>
/// 기본적인 탄환의 동작을 정의하며 유니티 내장 풀과 연동되는 컴포넌트입니다.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet01 : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;

    private BulletData _data;
    private bool _isInitialized = false;

    // --- 유니티 내장 풀 참조 ---
    private IObjectPool<Bullet01> _pool;
    // -------------------------

    private float _lifeTime = 5f;
    private float _spawnTime;

    /// <summary>
    /// 탄환에게 소속된 풀을 알려줍니다.
    /// </summary>
    public void SetPool(IObjectPool<Bullet01> pool)
    {
        _pool = pool;
    }

    /// <summary>
    /// 탄환을 초기화하고 수명 주기를 설정합니다.
    /// </summary>
    /// <param name="data">탄환 생성 데이터</param>
    public void Init(BulletData data)
    {
        _data = data;
        _spawnTime = Time.time;
        
        // 외형 및 크기 적용
        if (TryGetComponent<SpriteRenderer>(out var sr) && data.bulletSprite != null)
        {
            sr.sprite = data.bulletSprite;
        }
        transform.localScale = data.scale;

        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        // 수명 체크
        if (Time.time >= _spawnTime + _lifeTime)
        {
            ReturnToPool();
            return;
        }

        // 이동 처리
        transform.position += transform.up * (_data.speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(_data.damage);
                Debug.Log($"데미지 : {_data.damage}");
            }

            if (_data.pierceCount <= 0)
            {
                ReturnToPool();
            }
            else
            {
                _data.pierceCount--;
            }
        }
    }

    private void ReturnToPool()
    {
        _isInitialized = false;
        if (_pool != null)
        {
            // 유니티 내장 풀의 반납 방식
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject); // 풀이 없으면 파괴
        }
    }
}
