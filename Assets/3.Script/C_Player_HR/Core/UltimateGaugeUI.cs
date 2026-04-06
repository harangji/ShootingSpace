using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

/// <summary>
/// 필살기 게이지를 화면에 시각화하는 UI 클래스입니다.
/// </summary>
public class UltimateGaugeUI : MonoBehaviour
{
    [Title("UI References")]
    [SerializeField] private Slider gaugeSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject readyEffect; // 게이지가 가득 찼을 때 활성화될 이펙트/텍스트

    [Title("Logic Reference")]
    [SerializeField] private PlayerUltimateManager ultimateManager;

    private void Update()
    {
        if (ultimateManager == null) return;

        float ratio = ultimateManager.GaugeRatio;

        // 1. 슬라이더 업데이트
        if (gaugeSlider != null)
        {
            gaugeSlider.value = ratio;
        }

        // 2. 이미지 FillAmount 업데이트 (슬라이더 대신 이미지만 쓸 경우)
        if (fillImage != null)
        {
            fillImage.fillAmount = ratio;
        }

        // 3. 준비 완료 효과
        if (readyEffect != null)
        {
            readyEffect.SetActive(ultimateManager.CanUse);
        }
    }

    /// <summary>
    /// 수동으로 플레이어 매니저를 연결해야 할 때 사용합니다.
    /// </summary>
    public void Setup(PlayerUltimateManager manager)
    {
        ultimateManager = manager;
    }
}
