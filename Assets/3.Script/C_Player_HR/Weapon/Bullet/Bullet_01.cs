using UnityEngine;
using UnityEngine.Pool; // 유니티 내장 풀링 시스템 라이브러리 추가!

/// <summary>
/// 기본적인 탄환의 동작을 정의하며 유니티 내장 풀과 연동되는 컴포넌트입니다.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet_01 : MonoBehaviour, IPoolable<Bullet_01>
{
    [SerializeField] private LayerMask enemyLayer;

    private BulletData _data;
    private bool _isInitialized = false;

    // --- 유니티 내장 풀 참조 ---
    private IObjectPool<Bullet_01> _pool;
    // -------------------------

    private float _lifeTime = 5f;
    private float _spawnTime;

    /// <summary>
    /// 탄환에게 소속된 풀을 알려줍니다.
    /// </summary>
    public void SetPool(IObjectPool<Bullet_01> pool)
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
        // 이미 반납 처리 중인 경우 무시 (중복 실행 방지)
        if (!_isInitialized) return;

        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(_data.damage);
                Debug.Log($"데미지 : {_data.damage}");
            }

            // 분열 로직 추가
            if (_data.splitCount > 0)
            {
                HandleSplit();
            }

            // 도탄 로직 추가
            if (_data.bounceCount > 0)
            {
                if (HandleBounce(other))
                {
                    return; // 도탄 성공 시 아래의 관통/반납 로직을 건너뜁니다.
                }
            }

            if (_data.pierceCount <= 0)
            {
                ReturnToPool();
            }
            else
            {
                Debug.Log("관통됨");
                _data.pierceCount--;
            }
        }
    }

    private bool HandleBounce(Collider2D hitCollider)
    {
        float searchRadius = 10f;
        Collider2D[] others = Physics2D.OverlapCircleAll(transform.position, searchRadius, enemyLayer);
        
        Collider2D nearest = null;
        float minDistance = float.MaxValue;

        foreach (var col in others)
        {
            if (col == hitCollider) continue; // 방금 맞은 놈은 제외

            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = col;
            }
        }

        if (nearest != null)
        {
            // 새로운 목표를 향해 방향 전환
            Vector2 newDir = (nearest.transform.position - transform.position).normalized;
            transform.up = newDir;
            
            _data.bounceCount--;
            Debug.Log($"도탄됨! 남은 횟수: {_data.bounceCount}");
            return true;
        }

        return false; // 근처에 적이 없으면 도탄 실패
    }

    private void HandleSplit()
    {
        if (_pool == null) return;

        float spreadAngle = 30f; // 부채꼴 퍼짐 각도
        int count = _data.splitCount;
        
        // 진행 방향을 기준으로 각도 계산
        // 현재는 transform.up이 이동 방향입니다.
        Vector2 currentDir = transform.up;

        for (int i = 0; i < count; i++)
        {
            // 부채꼴 각도 분배 (2개일 경우 -15, +15 등)
            float angleOffset = (i - (count - 1) / 2f) * spreadAngle;
            Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);
            Vector2 splitDir = rotation * currentDir;

            // 새로운 탄환 데이터 생성
            BulletData splitData = _data;
            splitData.direction = splitDir;
            splitData.damage = Mathf.RoundToInt(_data.damage * _data.splitDamageMultiplier);
            splitData.scale = _data.scale * _data.splitScaleMultiplier;
            splitData.splitCount = 0; // 무한 분열 방지
            splitData.pierceCount = 0; // 분열탄은 관통하지 않게 설정 (취향에 따라 변경 가능)

            // 풀에서 새로운 탄환 가져오기
            Bullet_01 miniBullet = _pool.Get();
            if (miniBullet != null)
            {
                miniBullet.transform.position = transform.position;
                miniBullet.transform.up = splitDir;
                miniBullet.Init(splitData);
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
