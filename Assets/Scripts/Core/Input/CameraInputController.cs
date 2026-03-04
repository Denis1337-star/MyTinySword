using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class CameraInputController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0.01f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;

    [Header("Bounds")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Camera cam;

    private Vector2 lastTouchPosition;
    private bool isDragging;

    private float lastPinchDistance;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
        if (Touchscreen.current == null)
            return;

        var touches = Touchscreen.current.touches;

        // Ћюбое касание Ч отмен€ем Cinemachine-фокус
        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            var focus = FindAnyObjectByType<CameraFocusController>();
            if (focus != null)
                focus.CancelFocus();
        }

        //  ќдин палец Ч движение камеры
        if (touches.Count == 1 && touches[0].isInProgress)
        {
            Vector2 currentPos = touches[0].position.ReadValue();

            if (!isDragging)
            {
                lastTouchPosition = currentPos;
                isDragging = true;
                return;
            }

            Vector2 delta = currentPos - lastTouchPosition;
            MoveCamera(delta);

            lastTouchPosition = currentPos;
        }
        else
        {
            isDragging = false;
        }

        // ƒва пальца Ч зум
        if (touches.Count == 2 &&
            touches[0].isInProgress &&
            touches[1].isInProgress)
        {
            float currentDistance = Vector2.Distance(
                touches[0].position.ReadValue(),
                touches[1].position.ReadValue()
            );

            if (lastPinchDistance == 0)
            {
                lastPinchDistance = currentDistance;
                return;
            }

            float delta = currentDistance - lastPinchDistance;
            ZoomCamera(delta);

            lastPinchDistance = currentDistance;
        }
        else
        {
            lastPinchDistance = 0;
        }
    }

    private void MoveCamera(Vector2 delta)
    {
        Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * moveSpeed;
        Vector3 targetPos = transform.position + move;

        targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);

        transform.position = targetPos;
    }

    private void ZoomCamera(float delta)
    {
        cam.orthographicSize -= delta * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}
