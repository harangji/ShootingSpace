using UnityEngine;
using ShootingSpace.Core;

/// <summary>
/// 기본적인 체력 시스템을 가지며 데미지를 입었을 때 파괴되는 적 클래스입니다.
/// </summary>
public class SimpleEnemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 30;
    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log($"{gameObject.name}이(가) {damage}의 데미지를 입었습니다. (남은 체력: {_currentHealth})");

        // 체력이 0 이하가 되면 파괴 처리합니다.
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 적이 사망했을 때의 처리를 수행합니다.
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name}이(가) 파괴되었습니다.");
        
        Destroy(gameObject);
    }
}
