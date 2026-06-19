using DG.Tweening;
using UnityEngine;

/// <summary>
/// DOTween-анимация появления и скрытия UI-панелей.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public sealed class SimplePanelTween : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField, Min(0.01f)] private float _duration = 0.2f;
    [SerializeField, Min(0.01f)] private float _hiddenScale = 0.9f;
    [SerializeField] private Ease _showEase = Ease.OutBack;
    [SerializeField] private Ease _hideEase = Ease.InBack;

    public bool IsVisible => gameObject.activeSelf;

    private void OnDestroy()
    {
        KillTweens();
    }

    public void Show()
    {
        KillTweens();

        gameObject.SetActive(true);

        transform.localScale = Vector3.one * _hiddenScale;

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        transform
            .DOScale(Vector3.one, _duration)
            .SetEase(_showEase)
            .SetUpdate(true);

        _canvasGroup
            .DOFade(1f, _duration)
            .SetUpdate(true)
            .OnComplete(EnableInteraction);
    }

    public void Hide()
    {
        if (!IsVisible)
            return;

        KillTweens();

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        transform
            .DOScale(Vector3.one * _hiddenScale, _duration)
            .SetEase(_hideEase)
            .SetUpdate(true);

        _canvasGroup
            .DOFade(0f, _duration)
            .SetUpdate(true)
            .OnComplete(HideImmediate);
    }

    public void HideImmediate()
    {
        KillTweens();

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        transform.localScale = Vector3.one;

        gameObject.SetActive(false);
    }

    private void EnableInteraction()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    private void KillTweens()
    {
        transform.DOKill();
        _canvasGroup.DOKill();
    }
}
