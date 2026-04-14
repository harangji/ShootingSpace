using UnityEngine;
using System.Collections;

/// <summary>
/// 적의 공격 범위를 시각적으로 예고하는 인디케이터 클래스입니다.
/// </summary>
public class EnemyIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color normalColor = new Color(1, 0, 0, 0.3f);
    [SerializeField] private Color activeColor = new Color(1, 0, 0, 0.7f);

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 인디케이터를 활성화하고 설정합니다.
    /// </summary>
    /// <param name="position">위치</param>
    /// <param name="rotation">회전</param>
    /// <param name="scale">크기</param>
    public void Show(Vector2 position, Quaternion rotation, Vector2 scale)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
        
        if (spriteRenderer != null) spriteRenderer.color = normalColor;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 충전 진행률에 따라 색상을 변경합니다 (0 ~ 1).
    /// </summary>
    public void SetProgress(float ratio)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(normalColor, activeColor, ratio);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
