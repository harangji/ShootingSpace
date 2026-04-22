using UnityEngine;

/// <summary>
/// 에셋 메뉴를 통해 생성 가능한 탄환 관통 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "Pierce", menuName = "ShootingSpace/Augments/Pierce")]
public class PierceSO : ItemAugmentSO
{
    [Header("관통 설정")]
    [Tooltip("모든 탄환에 추가될 관통 횟수입니다.")]
    [SerializeField] private int extraPierceCount = 1;

    public override string GetDescription()
    {
        int current = extraPierceCount * level;
        int next = extraPierceCount * (level + 1);
        string lvText = IsMaxLevel ? "최대 레벨" : $"Lv.{level} -> Lv.{level + 1}";
        string nextEffect = IsMaxLevel ? "" : $"\n(다음: {next}회 관통)";
        
        return $"{description}\n[현재: {current}회 관통]\n<{lvText}>{nextEffect}";
    }

    /// <summary>
    /// 발사되는 모든 탄환에 지정된 횟수만큼 관통 능력을 부여합니다.
    /// </summary>
    /// <param name="context">발사 관련 컨텍스트 데이터</param>
    public override void ModifyFire(FireContext context)
    {
        if (context == null || context.bullets == null) return;

        for (int i = 0; i < context.bullets.Count; i++)
        {
            var bullet = context.bullets[i];
            // 레벨에 비례하여 관통 횟수 추가
            bullet.pierceCount += extraPierceCount * level;
            context.bullets[i] = bullet;
        }
    }
}
