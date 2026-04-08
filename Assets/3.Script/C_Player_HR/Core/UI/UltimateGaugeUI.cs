using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// OnGUI(IMGUI)를 사용하여 화면에 필살기 게이지를 시각적으로 표시하는 테스트용 UI 클래스입니다.
/// </summary>
public class UltimateGaugeUI : MonoBehaviour
{
    [Title("GUI Settings")]
    [SerializeField] private float xOffset = 20f;
    [SerializeField] private float yOffset = 20f;
    [SerializeField] private float width = 250f;
    [SerializeField] private float height = 30f;

    [Title("Logic Reference")]
    [SerializeField] private PlayerUltimateManager ultimateManager;

    private void OnGUI()
    {
        if (ultimateManager == null)
        {
            // 매니저가 연결되어 있지 않다면 씬에서 자동으로 찾아본다냥!
            ultimateManager = Object.FindFirstObjectByType<PlayerUltimateManager>();
            if (ultimateManager == null) return;
        }

        float ratio = ultimateManager.GaugeRatio;
        bool canUse = ultimateManager.CanUse;

        // 1. 전체 배경 박스
        Rect rect = new Rect(xOffset, yOffset, width, height);
        GUI.Box(rect, "");

        // 2. 게이지 바 (색상을 입혀서 겹쳐 그린다냥)
        Color oldColor = GUI.color;
        GUI.color = canUse ? Color.cyan : Color.yellow;
        
        // 게이지 길이에 맞춰 박스를 그린다냥
        GUI.Box(new Rect(xOffset, yOffset, width * ratio, height), "");
        GUI.color = oldColor;

        // 3. 중앙에 텍스트 표시
        string statusText = canUse ? "★ ULTIMATE READY (Mouse Right) ★" : $"Ultimate Gauge: {(ratio * 100):F0}%";
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        
        GUI.Label(rect, statusText, labelStyle);
    }

    /// <summary>
    /// 수동으로 플레이어 매니저를 연결해야 할 때 사용합니다.
    /// </summary>
    public void Setup(PlayerUltimateManager manager)
    {
        ultimateManager = manager;
    }
}
