using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

/// <summary>
/// 플레이어의 필살기 게이지를 관리하고 실행하는 컴포넌트입니다.
/// </summary>
public class PlayerUltimateManager : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, SerializeReference] private UltimateSkillBase currentSkill;
    [SerializeField] private float maxGauge = 100f;
    [SerializeField] private float autoChargePerSecond = 5f; // 초당 자동 충전량 (테스트용)

    [Title("Current State")]
    [SerializeField, ReadOnly] private float currentGauge = 0f;
    [SerializeField, ReadOnly] private bool isUltimateActive = false;

    private PlayerController _controller;

    public float GaugeRatio => Mathf.Clamp01(currentGauge / maxGauge);
    public bool CanUse => currentGauge >= maxGauge && !isUltimateActive;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (isUltimateActive) return;

        // 1. 자동 충전 로직 (실제 게임에서는 적 처치 시 등으로 변경 가능!)
        AddGauge(autoChargePerSecond * Time.deltaTime);

        // 2. 입력 감지 (우클릭 사용)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryUseUltimate();
        }
    }

    /// <summary>
    /// 외부(적 처치 등)에서 게이지를 추가할 때 사용합니다.
    /// </summary>
    public void AddGauge(float amount)
    {
        if (isUltimateActive) return;

        currentGauge = Mathf.Min(currentGauge + amount, maxGauge);
    }

    /// <summary>
    /// 무기들 중에서 장착된 필살기를 찾아 반환합니다.
    /// </summary>
    private UltimateSkillBase GetAvailableUltimate()
    {
        // 1. 만약 매니저에 직접 할당된 게 있다면 우선 사용
        if (currentSkill != null) return currentSkill;

        // 2. 장착된 무기들을 순회하며 필살기가 있는지 확인
        if (_controller != null && _controller.Equipment != null)
        {
            foreach (var weapon in _controller.Equipment.WeaponSlots)
            {
                if (weapon != null && weapon.ultimateSkill != null)
                {
                    return weapon.ultimateSkill;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 필살기 사용을 시도합니다.
    /// </summary>
    [Button("Force Use Ultimate (Debug)")]
    public void TryUseUltimate()
    {
        UltimateSkillBase skill = GetAvailableUltimate();

        if (skill == null)
        {
            Debug.LogWarning("[Ultimate] 장착된 무기에 필살기가 없습니다.");
            return;
        }

        if (currentGauge < skill.RequiredGauge)
        {
            Debug.Log($"[Ultimate] 게이지 부족: {currentGauge:F1}/{skill.RequiredGauge}");
            return;
        }

        // 필살기 실행
        ExecuteUltimate(skill);
    }

    private void ExecuteUltimate(UltimateSkillBase skill)
    {
        isUltimateActive = true;
        currentGauge = 0f; // 게이지 소모

        Debug.Log($"[Ultimate] 필살기 '{skill.SkillName}' 발동!!!");
        skill.Activate(_controller);

        // 지속 시간이 있는 경우 일정 시간 뒤에 비활성화 처리
        if (skill.Duration > 0)
        {
            // Deactivate 시 현재 실행 중인 스킬 정보를 전달하기 위해 코루틴 사용
            StartCoroutine(WaitAndDeactivate(skill));
        }
        else
        {
            isUltimateActive = false;
        }
    }

    private System.Collections.IEnumerator WaitAndDeactivate(UltimateSkillBase skill)
    {
        yield return new WaitForSeconds(skill.Duration);
        if (skill != null)
        {
            skill.Deactivate(_controller);
        }
        isUltimateActive = false;
        Debug.Log($"[Ultimate] 필살기 '{skill.SkillName}' 지속 시간 종료");
    }
}
