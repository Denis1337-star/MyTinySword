using UnityEngine;
using Zenject;

/// <summary>
/// Визуальный индикатор команды движения армии
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public sealed class MoveCommandIndicator : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField, Min(0.05f)] private float _lifeTime = 0.6f;
    [SerializeField, Min(0.01f)] private float _startScale = 0.75f;
    [SerializeField, Min(0.01f)] private float _endScale = 1.15f;

    private SpriteRenderer _spriteRenderer;
    private Color _startColor;
    private float _timer;
    private bool _isPlaying;
    private GameAudioService _audioService;

    [Inject]
    private void Construct(GameAudioService audioService)
    {
        _audioService = audioService;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startColor = _spriteRenderer.color;

        Hide();
    }

    private void Update()
    {
        if (!_isPlaying)
            return;

        _timer -= Time.deltaTime;

        float normalizedTime = 1f - Mathf.Clamp01(_timer / _lifeTime);

        UpdateScale(normalizedTime);
        UpdateAlpha(normalizedTime);

        if (_timer > 0f)
            return;

        Hide();
    }

    /// <summary>
    /// Показывает индикатор в мировой позиции
    /// </summary>
    public void Show(Vector2 worldPosition)
    {
        transform.position = new Vector3( worldPosition.x, worldPosition.y,
            transform.position.z);

        _timer = _lifeTime;
        _isPlaying = true;

        transform.localScale = Vector3.one * _startScale;

        _spriteRenderer.color = _startColor;
        _spriteRenderer.enabled = true;

        PlayMoveCommandSound();
    }

    private void UpdateScale(float normalizedTime)
    {
        float scale = Mathf.Lerp(_startScale, _endScale, normalizedTime);

        transform.localScale = Vector3.one * scale;
    }

    private void UpdateAlpha(float normalizedTime)
    {
        Color color = _startColor;
        color.a = Mathf.Lerp(_startColor.a, 0f, normalizedTime);

        _spriteRenderer.color = color;
    }

    private void Hide()
    {
        _isPlaying = false;
        _timer = 0f;

        if (_spriteRenderer != null)
            _spriteRenderer.enabled = false;
    }
    private void PlayMoveCommandSound()
    {
        if (_audioService == null)
            return;

        _audioService.PlayWorldSound(SoundId.MoveCommand, transform.position);
    }
}