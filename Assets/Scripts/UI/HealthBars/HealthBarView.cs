using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// UI-представление полоски здоровья над world-объектом.
/// Живёт внутри Screen Canvas, но следует за Transform в мире.
/// </summary>
public sealed class HealthBarView : MonoBehaviour
{
    [Header("UI")]
    [FormerlySerializedAs("fillImage")]
    [SerializeField] private Image _fillImage;

    private RectTransform _rectTransform;
    private RectTransform _canvasRectTransform;

    private Transform _target;
    private Camera _mainCamera;
    private Canvas _canvas;
    private Vector3 _worldOffset;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
    }

    public void Initialize(
        Transform target,
        Camera mainCamera,
        Canvas canvas,
        Color fillColor,
        Vector3 worldOffset)
    {
        _target = target;
        _mainCamera = mainCamera;
        _canvas = canvas;
        _worldOffset = worldOffset;

        _canvasRectTransform = _canvas != null
            ? _canvas.transform as RectTransform
            : null;

        if (_fillImage != null)
            _fillImage.color = fillColor;

        UpdatePosition();
    }

    public void SetFill(float normalizedValue)
    {
        if (_fillImage == null)
            return;

        _fillImage.fillAmount = Mathf.Clamp01(normalizedValue);
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (_target == null || _mainCamera == null || _canvas == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 worldPosition = _target.position + _worldOffset;
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z < 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        if (_rectTransform == null)
            return;

        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _rectTransform.position = screenPosition;
            return;
        }

        if (_canvasRectTransform == null)
            return;

        Camera canvasCamera = _canvas.worldCamera != null
            ? _canvas.worldCamera
            : _mainCamera;

        bool converted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,
            screenPosition,
            canvasCamera,
            out Vector2 localPoint);

        if (!converted)
            return;

        _rectTransform.localPosition = localPoint;
    }
}