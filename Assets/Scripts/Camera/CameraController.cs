using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Управление камерой: перемещение и zoom мыши.
/// </summary>
public sealed class CameraController : ValidatedMonoBehaviour
{
    private const float MouseDragStartThreshold = 25f;
    private const float MouseDragStartThresholdSqr = MouseDragStartThreshold * MouseDragStartThreshold;
    private const float MinMouseWheelDelta = 0.01f;

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CinemachineConfiner2D _confiner2D;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _moveSpeed = 0.001f;
    [SerializeField] private float _zoomSpeed = 0.01f;
    [SerializeField, Min(0.1f)] private float _minZoom = 3f;
    [SerializeField, Min(0.1f)] private float _maxZoom = 12f;

    private float _lastPinchDistance;

    private Vector2 _mousePressScreenPosition;
    private Vector2 _mousePreviousScreenPosition;

    private bool _hasMousePress;
    private bool _isMouseDragActive;
    private bool _mousePressStartedOverUi;
    private bool _inputEnabled = true;

    public bool IsDragging { get; private set; }

    public void SetInputEnabled(bool enabled)
    {
        _inputEnabled = enabled;

        if (!enabled)
            ResetMouseDragState();
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        ClampZoomValues();
    }

    private void Update()
    {
        HandleInput();
    }

    private void OnValidate()
    {
        ClampZoomValues();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _virtualCamera, nameof(_virtualCamera));
        valid &= ValidationUtility.IsAssigned(this, _confiner2D, nameof(_confiner2D));
        valid &= ValidationUtility.IsAssigned(this, _cameraTarget, nameof(_cameraTarget));

        return valid;
    }

    private void HandleInput()
    {
        if (!_inputEnabled)
            return;

        IsDragging = false;

        if (Touch.activeTouches.Count > 0)
        {
            ResetMouseDragState();
            HandleTouchInput();
            return;
        }

        _lastPinchDistance = 0f;

        HandleMouseInput();
    }

    private void HandleTouchInput()
    {
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

    private void HandleMouseInput()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            ResetMouseDragState();
            return;
        }

        Vector2 currentScreenPosition = mouse.position.ReadValue();

        HandleMouseWheelZoom(mouse);

        if (mouse.leftButton.wasPressedThisFrame)
            BeginMouseDrag(currentScreenPosition);

        if (mouse.leftButton.isPressed)
            ContinueMouseDrag(currentScreenPosition);

        if (mouse.leftButton.wasReleasedThisFrame)
            ResetMouseDragState();
    }

    private void BeginMouseDrag(Vector2 screenPosition)
    {
        _mousePressScreenPosition = screenPosition;
        _mousePreviousScreenPosition = screenPosition;

        _hasMousePress = true;
        _isMouseDragActive = false;
        _mousePressStartedOverUi = IsMousePointerOverUI();
    }

    private void ContinueMouseDrag(Vector2 currentScreenPosition)
    {
        if (!_hasMousePress)
            return;

        if (_mousePressStartedOverUi)
            return;

        Vector2 movementFromPress = currentScreenPosition - _mousePressScreenPosition;

        if (!_isMouseDragActive)
        {
            if (movementFromPress.sqrMagnitude <= MouseDragStartThresholdSqr)
            {
                _mousePreviousScreenPosition = currentScreenPosition;
                return;
            }

            _isMouseDragActive = true;
        }

        Vector2 delta = currentScreenPosition - _mousePreviousScreenPosition;
        _mousePreviousScreenPosition = currentScreenPosition;

        if (delta.sqrMagnitude <= 0f)
            return;

        MoveCamera(delta);
        IsDragging = true;
    }

    private void HandleMouseWheelZoom(Mouse mouse)
    {
        float scrollDelta = mouse.scroll.ReadValue().y;

        if (Mathf.Abs(scrollDelta) <= MinMouseWheelDelta)
            return;

        if (IsMousePointerOverUI())
            return;

        ApplyZoom(scrollDelta);
    }

    private void MoveCamera(Vector2 screenDelta)
    {
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

        float clampedZoom = Mathf.Clamp(
            targetZoom,
            _minZoom,
            _maxZoom);

        if (Mathf.Approximately(currentZoom, clampedZoom))
            return;

        _virtualCamera.m_Lens.OrthographicSize = clampedZoom;

        _confiner2D.InvalidateCache();
    }

    private float GetCurrentZoom()
    {
        return _virtualCamera.m_Lens.OrthographicSize;
    }

    private bool IsMousePointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void ResetMouseDragState()
    {
        _hasMousePress = false;
        _isMouseDragActive = false;
        _mousePressStartedOverUi = false;

        _mousePressScreenPosition = Vector2.zero;
        _mousePreviousScreenPosition = Vector2.zero;
    }

    private void ClampZoomValues()
    {
        _moveSpeed = Mathf.Max(0.0001f, _moveSpeed);
        _zoomSpeed = Mathf.Max(0.0001f, _zoomSpeed);

        _minZoom = Mathf.Max(0.1f, _minZoom);
        _maxZoom = Mathf.Max(_minZoom, _maxZoom);
    }
}