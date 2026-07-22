using UnityEngine;
using UnityEngine.Pool; // 유니티 내장 풀링 시스템 라이브러리 추가!
using ShootingSpace.Core;

/// <summary>
/// 기본적인 탄환의 동작을 정의하며 유니티 내장 풀과 연동되는 컴포넌트입니다.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet_05 : MonoBehaviour, IPoolable<Bullet_05>
{
    [SerializeField] private LayerMask enemyLayer;

    private BulletData _data;
    private bool _isInitialized = false;

    // --- 유니티 내장 풀 참조 ---
    private IObjectPool<Bullet_05> _pool;
    // -------------------------

    private float _lifeTime = 3f;
    private float _spawnTime;

    [Header("회전 탄환 설정")]
    [SerializeField] private float _rotationRadius = 2.0f;      // 회전 반경
    [SerializeField] private float _rotationSpeed = 360.0f;     // 회전 속도 (도/초)

    private Transform _playerTransform;                         // 플레이어의 Transform 참조
    private float _currentAngle;                                // 현재 회전 각도

    /// <summary>
    /// 탄환에게 소속된 풀을 알려줍니다.
    /// </summary>
    public void SetPool(IObjectPool<Bullet_05> pool)
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
        
        /* 외형 및 크기 적용 (기존 주석 처리된 코드 복원)
        if (TryGetComponent<SpriteRenderer>(out var sr) && data.bulletSprite != null)
        {
            sr.sprite = data.bulletSprite;
        }
        transform.localScale = data.scale;*/

        // 플레이어 Transform 찾기
        _playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (_playerTransform == null)
        {
            Debug.LogError("Bullet_05: 'Player' 태그를 가진 GameObject를 찾을 수 없습니다! 총알이 회전하지 않습니다.");
        }

        // 초기 각도를 랜덤으로 설정 (총알이 겹치지 않도록)
        //_currentAngle = Random.Range(0f, 360f);
        _currentAngle = 0f;
        _isInitialized = true;

        _rotationSpeed = 720f / (_data.spinSpeed * 2);
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

        // 회전 이동 처리
        if (_playerTransform != null)
        {
            _currentAngle += _rotationSpeed * Time.deltaTime; // 각도 증가
            float radians = _currentAngle * Mathf.Deg2Rad; // 라디안으로 변환

            Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * _rotationRadius;
            transform.position = _playerTransform.position + offset;
            // 총알이 항상 플레이어를 바라보게 하려면 이 줄 추가
            // transform.up = (transform.position - _playerTransform.position).normalized;
        }
        else
        {
            // 플레이어 Transform이 없으면 기존 직진 이동 (Fallback)
            transform.position += transform.up * (_data.speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 반납 처리 중인 경우 무시 (중복 실행 방지)
        if (!_isInitialized) return;

        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(_data.damage);
                Debug.Log($"데미지 : {_data.damage}");
            }


        }
    }

    private void ReturnToPool()
    {
        // 이미 반납 처리된 경우 중복 실행 방지
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
