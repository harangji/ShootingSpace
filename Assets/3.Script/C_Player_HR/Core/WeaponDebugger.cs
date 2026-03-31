using UnityEngine;
using UnityEngine.InputSystem;

namespace ShootingSpace.Core
{
    /// <summary>
    /// 무기 시스템 디버깅을 위한 클래스입니다.
    /// 1, 2, 3 키로 증강을 추가하고 0 키로 스탯을 확인합니다.
    /// </summary>
    public class WeaponDebugger : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Gun_01 targetWeapon;

        [Header("Test Augments")]
        [SerializeField] private AugmentSO augment1;
        [SerializeField] private AugmentSO augment2;
        [SerializeField] private AugmentSO augment3;

        private void Update()
        {
            // 0번 키: 현재 무기 스탯 출력
            if (Keyboard.current.digit0Key.wasPressedThisFrame)
            {
                PrintWeaponStats();
            }

            // 1, 2, 3번 키: 지정된 증강 추가
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ToggleAugment(augment1, 1);
            }
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ToggleAugment(augment2, 2);
            }
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ToggleAugment(augment3, 3);
            }
            }

            private void PrintWeaponStats()
            {
            if (targetWeapon != null)
            {
                Debug.Log(targetWeapon.GetDebugStats());
            }
            else
            {
                Debug.LogWarning("[Debugger] 대상 무기가 설정되지 않았다냥!");
            }
            }

            /// <summary>
            /// 증강을 토글(추가/삭제)합니다.
            /// </summary>
            private void ToggleAugment(AugmentSO augment, int slot)
            {
            if (targetWeapon == null || augment == null)
            {
                Debug.LogWarning($"[Debugger] {slot}번 슬롯 설정 확인해라냥!");
                return;
            }

            // 1. 먼저 삭제를 시도해본다냥! (성공하면 이미 있었다는 뜻)
            bool removed = targetWeapon.RemoveAugment(augment.augmentName);

            if (removed)
            {
                Debug.Log($"[Debugger] {slot}번 증강 토글 OFF: {augment.augmentName} 제거 완료냥!");
            }
            else
            {
                // 2. 삭제에 실패했다면 없다는 뜻이니 새로 추가한다냥!
                bool added = targetWeapon.AddAugment(augment);
                if (added)
                {
                    Debug.Log($"[Debugger] {slot}번 증강 토글 ON: {augment.augmentName} 추가 완료냥!");
                }
            }
            }
            }

}
