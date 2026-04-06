using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro; // TextMeshPro 필요

/// <summary>
/// 무기 및 증강 시스템 디버깅을 위한 클래스입니다.
/// 1, 2, 3 키로 증강을 추가하고 0 키로 스탯을 확인합니다.
/// 'U' 키로 증강 선택 UI를 띄워 테스트합니다.
/// </summary>
public class WeaponDebugger : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WeaponBase targetWeapon;
    [SerializeField] private AugmentLibrarySO augmentLibrary;

    [Header("UI Selection (New)")]
    [SerializeField] private AugmentSelectionUI uiHandler; // UI 핸들러로 변경

    private GameObject selectionPanel; // 내부 캐싱
    private Button[] choiceButtons;
    private TextMeshProUGUI[] choiceTexts;

    [Header("Test Augments (Quick Slots)")]
    [SerializeField] private AugmentSO augment1;
    [SerializeField] private AugmentSO augment2;
    [SerializeField] private AugmentSO augment3;

    private List<AugmentSO> currentChoices = new List<AugmentSO>();

    private void Awake()
    {
        if (uiHandler != null)
        {
            // 씬에서 캔버스 찾기
            Canvas canvas = Object.FindAnyObjectByType<Canvas>(); 

            if (canvas != null)
            {
                uiHandler.CreateUI(canvas);
                selectionPanel = uiHandler.SelectionPanel;
                choiceButtons = uiHandler.ChoiceButtons;
                choiceTexts = uiHandler.ChoiceTexts;

                // 버튼 리스너 연결
                if (choiceButtons != null)
                {
                    for (int i = 0; i < choiceButtons.Length; i++)
                    {
                        int index = i;
                        if (choiceButtons[i] != null)
                            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
                    }
                }
            }
            else
            {
                Debug.LogWarning("[Debugger] 씬에 Canvas가 없습니다! UI가 생성되지 않습니다.");
            }
        }
    }

    private void Update()
    {
        // 선택창이 떠있을 때는 키 입력 방지 (필요 시)
        if (selectionPanel != null && selectionPanel.activeSelf) return;

        // 0번 키: 현재 무기 스탯 출력
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            PrintWeaponStats();
        }

        // 'U' 키: 증강 선택창 띄우기 (테스트용)
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            ShowAugmentSelection();
        }

        // 1, 2, 3번 키: 지정된 증강 추가/제거 (토글)
        if (Keyboard.current.digit1Key.wasPressedThisFrame) ToggleAugment(augment1, 1);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ToggleAugment(augment2, 2);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ToggleAugment(augment3, 3);
    }

    /// <summary>
    /// AugmentSelector를 통해 3개의 증강을 뽑아 UI에 보여줍니다.
    /// </summary>
    private void ShowAugmentSelection()
    {
        if (selectionPanel == null || playerController == null || augmentLibrary == null)
        {
            Debug.LogError($"[Debugger] 필수 필드 누락: Panel({selectionPanel != null}), PC({playerController != null}), Lib({augmentLibrary != null})");
            return;
        }

        // 1. Selector로 증강 후보 3개 추출
        currentChoices = AugmentSelector.GetRandomAugments(playerController.Equipment, augmentLibrary, 3);
        Debug.Log($"[Debugger] {currentChoices.Count}개의 증강 후보를 뽑았습니다.");

        if (currentChoices.Count == 0)
        {
            Debug.LogWarning("[Debugger] 등장 가능한 증강이 하나도 없습니다! 슬롯이 꽉 찼거나 라이브러리가 비어있을 수 있습니다.");
            return;
        }

        // 2. UI 데이터 할당
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentChoices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                AugmentSO aug = currentChoices[i];
                
                // 텍스트 표시
                string augName = string.IsNullOrEmpty(aug.augmentName) ? aug.name : aug.augmentName;
                string info = $"<b>{augName}</b>\n{aug.description}";
                if (aug.type == AugmentType.WeaponUnique) info += $"\n<size=80%>[ID: {aug.targetWeaponID}]</size>";
                
                if (choiceTexts[i] != null)
                {
                    choiceTexts[i].text = info;
                    Debug.Log($"[Debugger] 버튼 {i}에 설정됨: {augName}");
                }
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

        // 3. 패널 활성화
        selectionPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    /// <summary>
    /// UI 버튼 클릭 시 호출됩니다.
    /// </summary>
    private void OnChoiceSelected(int index)
    {
        if (index >= currentChoices.Count) return;

        AugmentSO selected = currentChoices[index];
        Debug.Log($"<b>[Debugger]</b> '{selected.augmentName}' 선택됨! (Type: {selected.type})");

        // 타입에 따라 적용
        if (selected.type == AugmentType.GlobalItem)
        {
            playerController.EquipItem(selected as ItemAugmentSO);
            Debug.Log("[Debugger] 전역 아이템 장착 시도 완료!");
        }
        else if (selected.type == AugmentType.WeaponUnique)
        {
            bool found = false;
            foreach (var weapon in playerController.Equipment.WeaponSlots)
            {
                if (weapon != null && weapon.WeaponID == selected.targetWeaponID)
                {
                    weapon.AddAugment(selected);
                    Debug.Log($"[Debugger] 무기 '{weapon.WeaponID}'에 증강 추가 성공!");
                    found = true;
                    break;
                }
            }
            if (!found) Debug.LogWarning($"[Debugger] ID '{selected.targetWeaponID}'에 맞는 장착된 무기를 찾지 못했습니다.");
        }
        else if (selected.type == AugmentType.NewWeapon && selected is WeaponUnlockSO unlockSO)
        {
            if (unlockSO.weaponPrefab != null)
            {
                playerController.EquipWeapon(unlockSO.weaponPrefab, playerController.transform);
                Debug.Log($"[Debugger] 새로운 무기 '{unlockSO.weaponPrefab.name}' 장착 시도 완료!");
            }
            else
            {
                Debug.LogError($"[Debugger] 해금 증강 '{selected.augmentName}'에 무기 프리팹이 설정되지 않았습니다.");
            }
        }
        // UI 닫기
        selectionPanel.SetActive(false);
        Time.timeScale = 1f;
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
