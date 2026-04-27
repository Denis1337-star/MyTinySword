using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Поддерживает drag одним пальцем и zoom двумя пальцами
/// </summary>
public class CameraController : MonoBehaviour
{
    private const float DragStartThreshold = 8f;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 0.01f;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 0.02f;
    [SerializeField] private float _minZoom = 3f;
    [SerializeField] private float _maxZoom = 12f;

    private Vector2 _lastTouchPosition;
    private Vector2 _startTouchPosition;

    private bool _isDragging;

    public bool IsDragging => _isDragging;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        _isDragging = false;
    }

    private void OnValidate()
    {
        _moveSpeed = Mathf.Max(0f, _moveSpeed);
        _zoomSpeed = Mathf.Max(0f, _zoomSpeed);

        _minZoom = Mathf.Max(0.1f, _minZoom);

        if (_maxZoom < _minZoom)
            _maxZoom = _minZoom;
    }

    private void Update()
    {
        int touchCount = Touch.activeTouches.Count;

        if (touchCount == 0)
        {
            _isDragging = false;
            return;
        }

        if (touchCount == 1)
        {
            HandleSingleTouch(Touch.activeTouches[0]);
            return;
        }

        HandlePinchZoom();
    }

    private void HandleSingleTouch(Touch touch)
    {
        if (IsTouchOverUI(touch))
        {
            _isDragging = false;
            return;
        }

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            _startTouchPosition = touch.screenPosition;
            _lastTouchPosition = touch.screenPosition;
            _isDragging = false;
            return;
        }

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Moved)
            return;

        float movedFromStart = Vector2.Distance(
            _startTouchPosition,
            touch.screenPosition);

        if (movedFromStart < DragStartThreshold)
            return;

        _isDragging = true;

        Vector2 delta = touch.screenPosition - _lastTouchPosition;
        _lastTouchPosition = touch.screenPosition;

        MoveCamera(delta);
    }

    private void HandlePinchZoom()
    {
        Touch firstTouch = Touch.activeTouches[0];
        Touch secondTouch = Touch.activeTouches[1];

        if (IsTouchOverUI(firstTouch) || IsTouchOverUI(secondTouch))
        {
            _isDragging = false;
            return;
        }

        _isDragging = true;

        Vector2 firstPreviousPosition = firstTouch.screenPosition - firstTouch.delta;
        Vector2 secondPreviousPosition = secondTouch.screenPosition - secondTouch.delta;

        float previousDistance = Vector2.Distance(firstPreviousPosition, secondPreviousPosition);
        float currentDistance = Vector2.Distance(firstTouch.screenPosition, secondTouch.screenPosition);

        float difference = currentDistance - previousDistance;

        ZoomCamera(difference);
    }

    private void MoveCamera(Vector2 delta)
    {
        if (_virtualCamera == null)
            return;

        float zoomScale = _virtualCamera.m_Lens.OrthographicSize;

        Vector3 move = new(
            -delta.x * _moveSpeed * zoomScale,
            -delta.y * _moveSpeed * zoomScale,
            0f);

        _virtualCamera.transform.position += move;
    }

    private void ZoomCamera(float difference)
    {
        if (_virtualCamera == null)
            return;

        float size = _virtualCamera.m_Lens.OrthographicSize;
        size -= difference * _zoomSpeed;
        size = Mathf.Clamp(size, _minZoom, _maxZoom);

        _virtualCamera.m_Lens.OrthographicSize = size;
    }

    private bool IsTouchOverUI(Touch touch)
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject(touch.touchId);
    }
}