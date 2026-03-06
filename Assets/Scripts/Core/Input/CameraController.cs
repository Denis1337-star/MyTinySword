using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class CameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float moveSpeed = 0.003f;
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 12f;

    private Vector2 lastTouchPos;
    public bool dragging;
    private float lastPinchDist;

    private void OnEnable() => EnhancedTouchSupport.Enable();
    private void OnDisable() => EnhancedTouchSupport.Disable();

    private void Update()
    {
        if (Touch.activeTouches.Count == 0)
            return;

        var touches = Touch.activeTouches;

        // ÇÓÌ
        if (touches.Count >= 2)
        {
            Vector2 p0 = touches[0].screenPosition;
            Vector2 p1 = touches[1].screenPosition;

            float dist = Vector2.Distance(p0, p1);

            if (lastPinchDist > 0)
                Zoom(dist - lastPinchDist);

            lastPinchDist = dist;
            dragging = false;
            return;
        }

        lastPinchDist = 0;

        // ÏÀÍÎÐÈÌ
        if (touches.Count == 1)
        {
            var touch = touches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                lastTouchPos = touch.screenPosition;
                dragging = true;
            }

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && dragging)
            {
                Vector2 delta = touch.screenPosition - lastTouchPos;
                Move(delta);
                lastTouchPos = touch.screenPosition;
            }

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
                dragging = false;
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
    public bool IsDragging() => dragging;
}

