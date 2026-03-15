using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Cinemachine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class CameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 0.003f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 12f;

    private Vector2 lastTouchPos;
    private float lastPinchDist;
    private bool isDragging;

    public bool IsDragging => isDragging;

    private void Update()
    {
        if (virtualCamera == null)
            return;

        var touches = Touch.activeTouches;
        if (touches.Count == 0)
        {
            ResetTouchState();
            return;
        }

        if (touches.Count >= 2)
        {
            HandleZoom(touches);
            return;
        }

        HandlePan(touches[0]);
    }

    private void HandleZoom(System.Collections.Generic.IReadOnlyList<Touch> touches)
    {
        Vector2 p0 = touches[0].screenPosition;
        Vector2 p1 = touches[1].screenPosition;

        float currentDist = Vector2.Distance(p0, p1);

        if (lastPinchDist > 0f)
        {
            float delta = currentDist - lastPinchDist;
            Zoom(delta);
        }

        lastPinchDist = currentDist;
        isDragging = false;
    }

    private void HandlePan(Touch touch)
    {
        lastPinchDist = 0f;

        if (touch.phase == TouchPhase.Began)
        {
            lastTouchPos = touch.screenPosition;
            isDragging = false;
            return;
        }

        if (touch.phase == TouchPhase.Moved)
        {
            Vector2 delta = touch.screenPosition - lastTouchPos;

            if (delta.sqrMagnitude > 0.1f)
            {
                isDragging = true;
                Move(delta);
            }

            lastTouchPos = touch.screenPosition;
            return;
        }

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }

    private void Move(Vector2 delta)
    {
        float zoomFactor = virtualCamera.m_Lens.OrthographicSize;
        Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * moveSpeed * zoomFactor;

        virtualCamera.transform.position += move;
    }

    private void Zoom(float delta)
    {
        float size = virtualCamera.m_Lens.OrthographicSize;
        size -= delta * zoomSpeed;
        virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(size, minZoom, maxZoom);
    }

    private void ResetTouchState()
    {
        isDragging = false;
        lastPinchDist = 0f;
    }
}

