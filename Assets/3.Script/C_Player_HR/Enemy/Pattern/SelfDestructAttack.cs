using UnityEngine;

[CreateAssetMenu(fileName = "SelfDestructAttack", menuName = "ShootingSpace/Enemy/Strategies/SelfDestruct")]
public class SelfDestructAttack : AttackStrategyBase
{
    public override void Execute(EnemyBase enemy)
    {
        float distance = Vector2.Distance(enemy.transform.position, enemy.GetPlayerTransform().position);
        
        enemy.SetMoveSpeed(enemy.GetMoveSpeed() * 1.01f);
        enemy.GetSpriteRenderer().color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 5, 1));

        if (distance <= 0.5f)
        {
            Debug.Log($"[SelfDestruct] {enemy.name} 자폭했습니다! 💥");
            enemy.TakeDamage(9999);
        }
    }
}
