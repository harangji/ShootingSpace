using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

/// <summary>
/// 적의 피격 시 시각 효과(색상 변경)를 담당하는 컴포넌트입니다.
/// </summary>
public class EnemyHitEffect : MonoBehaviour
{
    [SerializeField, LabelText("스프라이트 렌더러")] private SpriteRenderer spriteRenderer;
    [SerializeField, LabelText("피격 색상")] private Color hitColor = Color.white;
    [SerializeField, LabelText("피격 효과 시간")] private float hitEffectDuration = 0.1f;

    private Color _originalColor;
    private Coroutine _hitEffectCoroutine;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
        if (spriteRenderer != null)
            _originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// 피격 효과를 재생합니다.
    /// </summary>
    public void PlayHitEffect()
    {
        if (spriteRenderer == null) return;

        if (_hitEffectCoroutine != null) StopCoroutine(_hitEffectCoroutine);
        _hitEffectCoroutine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitEffectDuration);
        spriteRenderer.color = _originalColor;
    }
}
