using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 무기 및 증강 시스템 디버깅을 위한 클래스입니다.
/// 1, 2, 3 키로 증강을 추가하고 0 키로 스탯을 확인합니다.
/// </summary>
public class WeaponDebugger : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WeaponBase targetWeapon;

    [Header("Test Augments")]
    [SerializeField] private AugmentSO augment1;
    [SerializeField] private AugmentSO augment2;
    [SerializeField] private AugmentSO augment3;

    private void Awake()
    {
        // 명시적으로 할당되지 않았을 경우 부모나 자신에게서 찾습니다.
        if (playerController == null) playerController = GetComponentInParent<PlayerController>();
        if (targetWeapon == null) targetWeapon = GetComponentInChildren<WeaponBase>();
    }

    private void Update()
    {
        // 0번 키: 현재 무기 스탯 출력
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            PrintWeaponStats();
        }

        // 1, 2, 3번 키: 지정된 증강 추가/제거 (토글)
        if (Keyboard.current.digit1Key.wasPressedThisFrame) ToggleAugment(augment1, 1);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ToggleAugment(augment2, 2);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ToggleAugment(augment3, 3);
    }

    private void PrintWeaponStats()
    {
        if (targetWeapon != null)
        {
            Debug.Log($"<b>[Debugger]</b> {targetWeapon.WeaponID} 스탯:\n{targetWeapon.GetDebugStats()}");
        }
        else
        {
            Debug.LogWarning("[Debugger] 대상 무기가 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 증강을 토글(추가/삭제)합니다. 타입에 따라 무기 혹은 플레이어에게 적용됩니다.
    /// </summary>
    private void ToggleAugment(AugmentSO augment, int slot)
    {
        if (augment == null)
        {
            Debug.LogWarning($"[Debugger] {slot}번 슬롯에 증강이 설정되지 않았습니다.");
            return;
        }

        if (augment.type == AugmentType.GlobalItem)
        {
            HandleGlobalItem(augment as ItemAugmentSO, slot);
        }
        else
        {
            HandleWeaponUnique(augment, slot);
        }
    }

    private void HandleGlobalItem(ItemAugmentSO item, int slot)
    {
        if (playerController == null)
        {
            Debug.LogError("[Debugger] PlayerController를 찾을 수 없어 전역 아이템을 적용할 수 없습니다.");
            return;
        }

        if (playerController.HasItem(item.augmentName))
        {
            playerController.UnequipItem(item.augmentName);
            Debug.Log($"[Debugger] {slot}번 전역 아이템 토글 OFF: {item.augmentName} 해제");
        }
        else
        {
            playerController.EquipItem(item);
            Debug.Log($"[Debugger] {slot}번 전역 아이템 토글 ON: {item.augmentName} 장착");
        }
    }

    private void HandleWeaponUnique(AugmentSO augment, int slot)
    {
        if (targetWeapon == null)
        {
            Debug.LogWarning("[Debugger] 대상 무기가 없어 고유 증강을 적용할 수 없습니다.");
            return;
        }

        // 1. 먼저 삭제를 시도합니다. (성공하면 이미 있었다는 뜻)
        bool removed = targetWeapon.RemoveAugment(augment.augmentName);

        if (removed)
        {
            Debug.Log($"[Debugger] {slot}번 고유 증강 토글 OFF: {augment.augmentName} 제거");
        }
        else
        {
            // 2. 삭제에 실패했다면 없다는 뜻이니 새로 추가합니다.
            bool added = targetWeapon.AddAugment(augment);
            if (added)
            {
                Debug.Log($"[Debugger] {slot}번 고유 증강 토글 ON: {augment.augmentName} 추가");
            }
        }
    }
}
