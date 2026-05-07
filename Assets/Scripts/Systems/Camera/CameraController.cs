using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// ”правл€ет ручным перемещением и zoom камеры через touch input
/// </summary>
public sealed class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _moveSpeed = 0.001f;
    [SerializeField] private float _zoomSpeed = 0.01f;
    [SerializeField, Min(0.1f)] private float _minZoom = 3f;
    [SerializeField, Min(0.1f)] private float _maxZoom = 12f;

    private float _lastPinchDistance;

    public bool IsDragging { get; private set; }

    private void Awake()
    {
        ClampZoomValues();
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void OnValidate()
    {
        ClampZoomValues();
    }

    private void HandleTouchInput()
    {
        IsDragging = false;

        var activeTouches = Touch.activeTouches;

        if (activeTouches.Count == 1)
        {
            HandleSingleTouch(activeTouches[0]);
            return;
        }

        if (activeTouches.Count >= 2)
        {
            HandlePinchZoom(activeTouches[0], activeTouches[1]);
            return;
        }

        _lastPinchDistance = 0f;
    }

    private void HandleSingleTouch(Touch touch)
    {
        if (TouchUtility.IsPointerOverUI(touch))
            return;

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            return;

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Moved)
            return;

        Vector2 delta = touch.delta;

        if (delta.sqrMagnitude <= 0f)
            return;

        MoveCamera(delta);
        IsDragging = true;
    }

    private void HandlePinchZoom(Touch firstTouch, Touch secondTouch)
    {
        if (TouchUtility.IsPointerOverUI(firstTouch) || TouchUtility.IsPointerOverUI(secondTouch))
        {
            _lastPinchDistance = 0f;
            return;
        }

        Vector2 firstPosition = firstTouch.screenPosition;
        Vector2 secondPosition = secondTouch.screenPosition;

        float currentDistance = Vector2.Distance(firstPosition, secondPosition);

        if (_lastPinchDistance <= 0f)
        {
            _lastPinchDistance = currentDistance;
            return;
        }

        float distanceDelta = currentDistance - _lastPinchDistance;
        _lastPinchDistance = currentDistance;

        ApplyZoom(distanceDelta);
    }

    private void MoveCamera(Vector2 screenDelta)
    {
        if (_cameraTarget == null)
            return;

        float zoomMultiplier = GetCurrentZoom();

        Vector3 moveDelta = new(
            -screenDelta.x * _moveSpeed * zoomMultiplier,
            -screenDelta.y * _moveSpeed * zoomMultiplier,
            0f);

        _cameraTarget.position += moveDelta;
    }

    private void ApplyZoom(float distanceDelta)
    {
        float currentZoom = _virtualCamera.m_Lens.OrthographicSize;
        float targetZoom = currentZoom - distanceDelta * _zoomSpeed;

        _virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(
            targetZoom,
            _minZoom,
            _maxZoom);
    }

    private float GetCurrentZoom()
    {
        return _virtualCamera.m_Lens.OrthographicSize;
    }

    private void ClampZoomValues()
    {
        _moveSpeed = Mathf.Max(0.0001f, _moveSpeed);
        _zoomSpeed = Mathf.Max(0.0001f, _zoomSpeed);

        _minZoom = Mathf.Max(0.1f, _minZoom);
        _maxZoom = Mathf.Max(_minZoom, _maxZoom);
    }
}