using DG.Tweening;
using UnityEngine;

/// <summary>
/// DOTween анимация названия игры в главном меню
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public sealed class MainMenuTitleTween : MonoBehaviour
{
    [Header("Intro Animation")]
    [SerializeField, Min(0.01f)] private float _showDuration = 0.45f;
    [SerializeField, Min(0.01f)] private float _startScale = 0.85f;
    [SerializeField] private Ease _showEase = Ease.OutBack;

    [Header("Idle Animation")]
    [SerializeField] private bool _playIdleAnimation = true;
    [SerializeField, Min(0.01f)] private float _idleScale = 1.03f;
    [SerializeField, Min(0.1f)] private float _idleDuration = 1.4f;

    private CanvasGroup _canvasGroup;
    private Sequence _sequence;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        PrepareInitialState();
    }

    private void Start()
    {
        PlayIntro();
    }

    private void OnDestroy()
    {
        KillAnimation();
    }

    private void PrepareInitialState()
    {
        KillAnimation();

        _canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * _startScale;
    }

    private void PlayIntro()
    {
        KillAnimation();

        _sequence = DOTween.Sequence();
        _sequence.SetUpdate(true);

        _sequence.Join(
            _canvasGroup
                .DOFade(1f, _showDuration));

        _sequence.Join(
            transform
                .DOScale(Vector3.one, _showDuration)
                .SetEase(_showEase));

        if (_playIdleAnimation)
            _sequence.OnComplete(PlayIdleAnimation);
    }

    private void PlayIdleAnimation()
    {
        KillAnimation();

        _sequence = DOTween.Sequence();
        _sequence.SetUpdate(true);
        _sequence.SetLoops(-1, LoopType.Yoyo);

        _sequence.Append(
            transform
                .DOScale(Vector3.one * _idleScale, _idleDuration)
                .SetEase(Ease.InOutSine));
    }

    private void KillAnimation()
    {
        if (_sequence != null)
        {
            _sequence.Kill();
            _sequence = null;
        }

        transform.DOKill();
        _canvasGroup?.DOKill();
    }
}