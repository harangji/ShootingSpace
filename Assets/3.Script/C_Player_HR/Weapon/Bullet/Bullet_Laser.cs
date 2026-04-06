using UnityEngine;
using UnityEngine.Pool;
using ShootingSpace.Core;

/// <summary>
/// 매우 빠른 속도로 날아가며 관통 성능이 뛰어난 레이저 탄환입니다.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Bullet_Laser : MonoBehaviour, IPoolable<Bullet_Laser>
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float rotationSpeed = 0f; // 레이저는 보통 자전하지 않음

    private BulletData _data;
    private bool _isInitialized = false;

    // --- 유니티 내장 풀 참조 ---
    private IObjectPool<Bullet_Laser> _pool;
    // -------------------------

    private float _lifeTime = 3f; // 레이저는 보통 수명이 짧고 빠름
    private float _spawnTime;

    public void SetPool(IObjectPool<Bullet_Laser> pool)
    {
        _pool = pool;
    }

    public void Init(BulletData data)
    {
        _data = data;
        _spawnTime = Time.time;
        
        // 레이저의 진행 방향 설정
        transform.up = data.direction;

        // 외형 및 크기 적용
        if (TryGetComponent<SpriteRenderer>(out var sr) && data.bulletSprite != null)
        {
            sr.sprite = data.bulletSprite;
        }
        
        // 레이저 특유의 길쭉한 형태를 위해 Y축 스케일을 조금 더 키워줄 수도 있습니다.
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

        // 이동 처리 (매우 빠른 직선 이동)
        transform.position += transform.up * (_data.speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isInitialized) return;

        // 적 레이어와 충돌했는지 확인
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(_data.damage);
            }

            // 레이저의 핵심: 기본적으로 높은 관통력
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
        if (!_isInitialized) return;

        _isInitialized = false;
        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
