using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 플레이어 시스템의 중앙 컨트롤러입니다. 
/// 이동, 스탯, 장비 컴포넌트를 조정하고 전체 상태를 관리합니다.
/// </summary>
public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStatsManager stats;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerEquipment equipment;
    
    [Title("피격 효과")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitDuration = 0.1f;
    
    private Color _originalColor;
    private Coroutine _hitCoroutine;

    [SerializeField] private Collider2D playerCollider;

    public Collider2D PlayerCollider => playerCollider;

    public PlayerEquipment Equipment => equipment;

    private void Awake()
    {
        // 초기화
        equipment.Initialize();
        RefreshAllStats();
        
        if (playerSprite != null) _originalColor = playerSprite.color;
        stats.OnDamaged += PlayHitEffect;
    }

    private void OnDestroy()
    {
        stats.OnDamaged -= PlayHitEffect;
    }

    private void PlayHitEffect()
    {
        if (playerSprite == null) return;
        if (_hitCoroutine != null) StopCoroutine(_hitCoroutine);
        _hitCoroutine = StartCoroutine(HitEffectRoutine());
    }

    private System.Collections.IEnumerator HitEffectRoutine()
    {
        playerSprite.color = hitColor;
        yield return new WaitForSeconds(hitDuration);
        playerSprite.color = _originalColor;
    }

    /// <summary>
    /// 플레이어 스탯과 모든 무기 스탯을 최신 상태로 동기화합니다.
    /// </summary>
    [ContextMenu("Refresh All Stats")]
    public void RefreshAllStats()
    {
        // 1. 플레이어 스탯 갱신
        stats.Refresh(equipment.ItemSlots);

        // 2. 모든 무기에 플레이어 컨텍스트 및 아이템 효과 전달
        foreach (var weapon in equipment.WeaponSlots)
        {
            if (weapon != null)
            {
                weapon.RefreshWeaponStats(stats.CurrentContext, equipment.ItemSlots);
            }
        }

        Debug.Log("[PlayerController] 모든 시스템 스탯 동기화 완료");
    }

    /// <summary>
    /// 새로운 아이템을 장착하고 스탯을 갱신합니다.
    /// </summary>
    public void EquipItem(ItemAugmentSO augment)
    {
        if (equipment.EquipItem(augment))
        {
            RefreshAllStats();
        }
    }

    /// <summary>
    /// 아이템을 해제하고 스탯을 갱신합니다.
    /// </summary>
    public void UnequipItem(string augmentName)
    {
        if (equipment.UnequipItem(augmentName))
        {
            RefreshAllStats();
        }
    }

    /// <summary>
    /// 전역 아이템 증강이 장착되어 있는지 확인합니다.
    /// </summary>
    public bool HasItem(string augmentName)
    {
        return equipment.HasItem(augmentName);
    }

    /// <summary>
    /// 새로운 무기를 장착하고 스탯을 갱신합니다.
    /// </summary>
    public bool EquipWeapon(WeaponBase weaponPrefab, Transform mountPoint)
    {
        if (equipment.EquipWeapon(weaponPrefab, mountPoint))
        {
            RefreshAllStats();
            return true;
        }
        return false;
    }

    public void TakeDamage(int damage)
    {
        stats.Health.Decrease(damage);
    }
}
