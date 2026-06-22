using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Двигает и масштабирует карту дерева развития внутри viewport.
/// Работает через отдельную DragSurface, чтобы кнопки нод не конфликтовали с перемещением карты.
/// </summary>
public sealed class TechTreeMapPanController : ValidatedMonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler
{
    [Header("Content")]
    [SerializeField] private RectTransform _mapContent;

    [Header("Pan Limits")]
    [SerializeField] private Vector2 _minPosition = new(-1000f, -700f);
    [SerializeField] private Vector2 _maxPosition = new(1000f, 700f);

    [Header("Zoom")]
    [SerializeField, Min(0.1f)] private float _minZoom = 0.75f;
    [SerializeField, Min(0.1f)] private float _maxZoom = 1.35f;
    [SerializeField, Min(0.01f)] private float _zoomSpeed = 0.1f;

    public event Action DragStarted;

    protected override bool ValidateInternal()
    {
        return ValidationUtility.IsAssigned(this, _mapContent, nameof(_mapContent));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragStarted?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 nextPosition = _mapContent.anchoredPosition + eventData.delta;

        _mapContent.anchoredPosition = ClampPosition(nextPosition);
    }

    public void OnScroll(PointerEventData eventData)
    {
        float zoomDelta = eventData.scrollDelta.y * _zoomSpeed;
        float currentZoom = _mapContent.localScale.x;
        float nextZoom = Mathf.Clamp(currentZoom + zoomDelta, _minZoom, _maxZoom);

        ApplyZoom(nextZoom);
    }

    public void ResetPosition()
    {
        _mapContent.anchoredPosition = Vector2.zero;
        ApplyZoom(1f);
    }

    private void ApplyZoom(float zoom)
    {
        _mapContent.localScale = Vector3.one * zoom;
        _mapContent.anchoredPosition = ClampPosition(_mapContent.anchoredPosition);
    }

    private Vector2 ClampPosition(Vector2 position)
    {
        position.x = Mathf.Clamp(position.x, _minPosition.x, _maxPosition.x);
        position.y = Mathf.Clamp(position.y, _minPosition.y, _maxPosition.y);

        return position;
    }
}