using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootAttack", menuName = "ShootingSpace/Enemy/Strategies/Shoot")]
public class ShootAttack : AttackStrategyBase
{
    [Title("사격 세부 설정")]
    [SerializeField] private float fireRate = 2f;
    private float _lastFireTime;

    public override void Execute(EnemyBase enemy)
    {
        if (Time.time < _lastFireTime + fireRate) return;

        Transform player = enemy.GetPlayerTransform();
        if (player == null) return;

        Debug.Log($"[ShootAttack] {enemy.name} 발사!");
        _lastFireTime = Time.time;
    }
}
