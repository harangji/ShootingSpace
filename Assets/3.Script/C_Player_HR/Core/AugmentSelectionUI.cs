using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 증강 선택 UI를 코드로 생성하고 관리하는 클래스입니다.
/// </summary>
public class AugmentSelectionUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color panelColor = new Color(0, 0, 0, 0.9f);
    [SerializeField] private Color buttonColor = new Color(0.15f, 0.15f, 0.15f, 1f);

    [Header("Font Settings")]
    [Tooltip("한글을 지원하는 TMP Font Asset을 할당해주세요. 비어있으면 기본 폰트를 사용합니다.")]
    [SerializeField] private TMP_FontAsset fontAsset;

    public GameObject SelectionPanel { get; private set; }
    public Button[] ChoiceButtons { get; private set; }
    public TextMeshProUGUI[] ChoiceTexts { get; private set; }

    public void CreateUI(Canvas canvas)
    {
        if (SelectionPanel != null) return;

        // 1. 배경 패널 생성
        SelectionPanel = CreateUIObject("AugmentSelectionPanel", canvas.transform);
        RectTransform panelRect = SelectionPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        SelectionPanel.AddComponent<Image>().color = panelColor;

        // 2. 타이틀 텍스트
        GameObject titleObj = CreateUIObject("Title", SelectionPanel.transform);
        var titleText = titleObj.AddComponent<TextMeshProUGUI>();
        SetupTMP(titleText, "증강 선택", 54, Color.yellow);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 350);
        titleRect.sizeDelta = new Vector2(800, 100);

        // 3. 버튼 레이아웃 그룹
        GameObject buttonGroup = CreateUIObject("ButtonGroup", SelectionPanel.transform);
        RectTransform groupRect = buttonGroup.GetComponent<RectTransform>();
        groupRect.sizeDelta = new Vector2(1200, 450); // 가로 폭을 넉넉히 잡음
        groupRect.anchoredPosition = Vector2.zero;
        
        HorizontalLayoutGroup layout = buttonGroup.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 50;
        layout.childAlignment = TextAnchor.MiddleCenter; // 중앙 정렬 추가
        layout.childControlWidth = false; // 너비를 직접 제어하기 위해 false
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false; // 버튼이 적을 때 강제로 늘어나지 않게 함
        layout.childForceExpandHeight = true;

        // 4. 버튼 3개 생성
        ChoiceButtons = new Button[3];
        ChoiceTexts = new TextMeshProUGUI[3];

        for (int i = 0; i < 3; i++)
        {
            GameObject btnObj = CreateUIObject($"ChoiceButton_{i}", buttonGroup.transform);
            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(350, 400); // 기본 버튼 크기 설정

            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = buttonColor;
            btnImg.type = Image.Type.Sliced;
            
            Button btn = btnObj.AddComponent<Button>();
            ChoiceButtons[i] = btn;

            // 버튼 내용물 레이아웃 (텍스트 여백용)
            GameObject txtObj = CreateUIObject("TextContent", btnObj.transform);
            var txt = txtObj.AddComponent<TextMeshProUGUI>();
            SetupTMP(txt, $"Choice {i}", 28, Color.white);
            
            RectTransform txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = new Vector2(30, 30);
            txtRect.offsetMax = new Vector2(-30, -30);

            ChoiceTexts[i] = txt;
        }

        SelectionPanel.SetActive(false);
    }

    private void SetupTMP(TextMeshProUGUI tmp, string content, float size, Color color)
    {
        tmp.text = content;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.textWrappingMode = TextWrappingModes.Normal; 
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        tmp.raycastTarget = false;

        if (fontAsset != null)
        {
            tmp.font = fontAsset;
        }
        else
        {
            tmp.font = TMP_Settings.defaultFontAsset;
        }
    }

    private GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }
}
