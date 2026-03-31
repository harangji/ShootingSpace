using UnityEngine;

/// <summary>
/// 에셋 메뉴를 통해 생성 가능한 탄환 관통 증강입니다.
/// </summary>
[CreateAssetMenu(fileName = "Pierce", menuName = "ShootingSpace/Augments/Pierce")]
public class PierceSO : AugmentSO
{
    [Header("관통 설정")]
    [Tooltip("모든 탄환에 추가될 관통 횟수입니다.")]
    [SerializeField] private int extraPierceCount = 1;

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
            bullet.pierceCount += extraPierceCount;
            context.bullets[i] = bullet;
        }
    }
}
