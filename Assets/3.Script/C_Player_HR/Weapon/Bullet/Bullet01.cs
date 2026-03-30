using UnityEngine;
using ShootingSpace.Core;

/// <summary>
/// 기본적인 탄환의 동작을 정의하는 컴포넌트입니다.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet01 : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;

    private BulletData _data;
    private bool _isInitialized = false;

    /// <summary>
    /// 탄환을 초기화하고 수명 주기를 설정합니다.
    /// </summary>
    /// <param name="data">탄환 생성 데이터</param>
    public void Init(BulletData data)
    {
        _data = data;
        _isInitialized = true;
        
        // 5초 뒤 자동 제거
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        if (!_isInitialized) return;

        // 자신의 Up 방향(Y축)으로 이동 처리
        transform.position += transform.up * (_data.speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy 레이어와 충돌 시 데미지 처리 및 소멸 로직
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            // 충돌한 객체에서 IDamageable 인터페이스가 있는지 확인합니다.
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(_data.damage);
            }

            // 관통 횟수가 없으면 소멸
            if (_data.pierceCount <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                _data.pierceCount--;
            }
        }
    }
}
