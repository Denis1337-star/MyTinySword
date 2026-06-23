using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Двигает и масштабирует карту дерева развития внутри viewport.
/// Поддерживает drag мышью, колесо мыши и pinch zoom двумя пальцами.
/// </summary>
public sealed class TechTreeMapPanController : ValidatedMonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler
{
    [Header("Content")]
    [SerializeField] private RectTransform _mapContent;

    [Header("Pan Limits")]
    [SerializeField] private Vector2 _minPosition = new(-1000f, -500f);
    [SerializeField] private Vector2 _maxPosition = new(1000f, 500f);

    [Header("Zoom")]
    [SerializeField, Min(0.1f)] private float _minZoom = 0.75f;
    [SerializeField, Min(0.1f)] private float _maxZoom = 1.35f;
    [SerializeField, Min(0.01f)] private float _zoomSpeed = 0.1f;
    [SerializeField, Min(0.001f)] private float _pinchZoomSpeed = 0.005f;

    public event Action DragStarted;

    private bool _pinching;
    private float _previousPinchDistance;

    protected override bool ValidateInternal()
    {
        return ValidationUtility.IsAssigned(this, _mapContent, nameof(_mapContent));
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        HandleTouchZoom();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Touch.activeTouches.Count >= 2)
            return;

        DragStarted?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Touch.activeTouches.Count >= 2)
            return;

        Vector2 nextPosition = _mapContent.anchoredPosition + eventData.delta;
        _mapContent.anchoredPosition = ClampPosition(nextPosition);
    }

    public void OnScroll(PointerEventData eventData)
    {
        float zoomDelta = eventData.scrollDelta.y * _zoomSpeed;
        ApplyZoom(GetCurrentZoom() + zoomDelta);
    }

    public void ResetPosition()
    {
        _mapContent.anchoredPosition = Vector2.zero;
        ApplyZoom(1f);
    }

    private void HandleTouchZoom()
    {
        var activeTouches = Touch.activeTouches;

        if (activeTouches.Count < 2)
        {
            _pinching = false;
            return;
        }

        Touch firstTouch = activeTouches[0];
        Touch secondTouch = activeTouches[1];

        float currentDistance = Vector2.Distance(
            firstTouch.screenPosition,
            secondTouch.screenPosition);

        if (!_pinching)
        {
            _pinching = true;
            _previousPinchDistance = currentDistance;
            DragStarted?.Invoke();
            return;
        }

        float distanceDelta = currentDistance - _previousPinchDistance;
        ApplyZoom(GetCurrentZoom() + distanceDelta * _pinchZoomSpeed);

        _previousPinchDistance = currentDistance;
    }

    private float GetCurrentZoom()
    {
        return _mapContent.localScale.x;
    }

    private void ApplyZoom(float zoom)
    {
        float clampedZoom = Mathf.Clamp(zoom, _minZoom, _maxZoom);

        _mapContent.localScale = Vector3.one * clampedZoom;
        _mapContent.anchoredPosition = ClampPosition(_mapContent.anchoredPosition);
    }

    private Vector2 ClampPosition(Vector2 position)
    {
        position.x = Mathf.Clamp(position.x, _minPosition.x, _maxPosition.x);
        position.y = Mathf.Clamp(position.y, _minPosition.y, _maxPosition.y);

        return position;
    }
}