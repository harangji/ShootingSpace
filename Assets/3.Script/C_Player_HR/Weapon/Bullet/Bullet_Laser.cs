using UnityEngine;
using UnityEngine.Pool;
using ShootingSpace.Core;
using Sirenix.OdinInspector;

/// <summary>
/// 매우 빠른 속도로 날아가며 관통 성능이 뛰어난 레이저 탄환입니다.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Bullet_Laser : MonoBehaviour, IPoolable<Bullet_Laser>
{
    [Title("탄환 설정")]
    [LabelText("충돌 대상 레이어")]
    [SerializeField] private LayerMask enemyLayer;

    [Title("실시간 상태")]
    [ReadOnly, LabelText("현재 데이터")]
    [SerializeField] private BulletData _data;
    
    private bool _isInitialized = false;
    private IObjectPool<Bullet_Laser> _pool;
    private float _lifeTime = 3f;
    private float _spawnTime;

    public void SetPool(IObjectPool<Bullet_Laser> pool) => _pool = pool;

    public void Init(BulletData data)
    {
        _data = data;
        _spawnTime = Time.time;
        transform.up = data.direction;

        if (TryGetComponent<SpriteRenderer>(out var sr) && data.bulletSprite != null)
            sr.sprite = data.bulletSprite;
        
        transform.localScale = data.scale;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        if (Time.time >= _spawnTime + _lifeTime)
        {
            ReturnToPool();
            return;
        }

        transform.position += transform.up * (_data.speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isInitialized) return;

        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(_data.damage);

            if (_data.pierceCount <= 0)
                ReturnToPool();
            else
                _data.pierceCount--;
        }
    }

    private void ReturnToPool()
    {
        if (!_isInitialized) return;
        _isInitialized = false;
        if (_pool != null) _pool.Release(this);
        else Destroy(gameObject);
    }
}
