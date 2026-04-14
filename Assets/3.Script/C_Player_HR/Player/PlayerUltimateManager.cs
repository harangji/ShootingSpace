using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

/// <summary>
/// 플레이어의 필살기 게이지를 관리하고 실행하는 컴포넌트입니다.
/// </summary>
public class PlayerUltimateManager : MonoBehaviour
{
    [TitleGroup("필살기 구성", "필살기 스킬과 게이지의 기본 설정을 관리합니다.", alignment: TitleAlignments.Left)]
    [BoxGroup("필살기 구성/기본 설정")]
    [LabelText("현재 장착된 필살기"), SerializeReference]
    [Tooltip("수동으로 할당하거나, 무기에 장착된 필살기가 자동으로 사용됩니다.")]
    private UltimateSkillBase currentSkill;

    [BoxGroup("필살기 구성/기본 설정")]
    [LabelText("최대 게이지 수치"), SuffixLabel("Point")]
    [Tooltip("필살기를 사용하기 위해 가득 채워야 하는 목표치입니다.")]
    [SerializeField] private float maxGauge = 100f;

    [BoxGroup("필살기 구성/기본 설정")]
    [LabelText("초당 자동 충전량"), SuffixLabel("P/sec")]
    [Tooltip("적을 처치하지 않아도 매초 자동으로 차오르는 게이지 양입니다.")]
    [SerializeField] private float autoChargePerSecond = 5f;

    [TitleGroup("실시간 상태", "현재 필살기 게이지의 충전 상태와 활성화 여부입니다.")]
    [ShowInInspector, LabelText("현재 충전 상태"), ProgressBar(0, "maxGauge", ColorGetter = "GetGaugeColor")]
    private float currentGauge = 0f;

    [ShowInInspector, LabelText("필살기 가동 여부"), GUIColor("GetActiveColor")]
    [InfoBox("필살기가 활성화된 상태에서는 게이지가 충전되지 않습니다. 🐾", InfoMessageType.None, "isUltimateActive")]
    private bool isUltimateActive = false;

    private PlayerController _controller;

    public float GaugeRatio => Mathf.Clamp01(currentGauge / maxGauge);
    public bool CanUse => currentGauge >= maxGauge && !isUltimateActive;

    private Color GetGaugeColor(float value) => value >= maxGauge ? Color.cyan : Color.yellow;
    private Color GetActiveColor() => isUltimateActive ? Color.green : Color.white;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (isUltimateActive) return;

        // 1. 자동 충전 로직
        AddGauge(autoChargePerSecond * Time.deltaTime);

        // 2. 입력 감지 (우클릭 사용)
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryUseUltimate();
        }
    }

    /// <summary>
    /// 외부(적 처치 등)에서 게이지를 추가할 때 호출합니다.
    /// </summary>
    public void AddGauge(float amount)
    {
        if (isUltimateActive) return;
        currentGauge = Mathf.Min(currentGauge + amount, maxGauge);
    }

    private UltimateSkillBase GetAvailableUltimate()
    {
        if (currentSkill != null) return currentSkill;

        if (_controller != null && _controller.Equipment != null)
        {
            foreach (var weapon in _controller.Equipment.WeaponSlots)
            {
                if (weapon != null && weapon.ultimateSkill != null)
                    return weapon.ultimateSkill;
            }
        }
        return null;
    }

    [TitleGroup("테스트 및 디버그")]
    [Button("필살기 강제 발동", ButtonSizes.Large), GUIColor(0, 1, 1)]
    public void TryUseUltimate()
    {
        UltimateSkillBase skill = GetAvailableUltimate();

        if (skill == null)
        {
            Debug.LogWarning("[Ultimate] 사용할 수 있는 필살기가 없습니다.");
            return;
        }

        if (currentGauge < skill.RequiredGauge)
        {
            Debug.Log($"[Ultimate] 게이지 부족: {currentGauge:F1}/{skill.RequiredGauge}");
            return;
        }

        ExecuteUltimate(skill);
    }

    private void ExecuteUltimate(UltimateSkillBase skill)
    {
        isUltimateActive = true;
        currentGauge = 0f;

        Debug.Log($"[Ultimate] 필살기 '{skill.SkillName}' 발동!!!");
        skill.Activate(_controller);

        if (skill.Duration > 0)
            StartCoroutine(WaitAndDeactivate(skill));
        else
            isUltimateActive = false;
    }

    private System.Collections.IEnumerator WaitAndDeactivate(UltimateSkillBase skill)
    {
        yield return new WaitForSeconds(skill.Duration);
        if (skill != null) skill.Deactivate(_controller);
        isUltimateActive = false;
        Debug.Log($"[Ultimate] 필살기 '{skill.SkillName}' 지속 시간 종료");
    }
}
