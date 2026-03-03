using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Cinemachine;

public class CameraInputController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.02f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;

    private Camera mainCamera;
    private Vector3 lastWorldPos;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
        mainCamera = Camera.main;

        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
        var touches = Touch.activeTouches;

        // ===== DRAG (1 палец) =====
        if (touches.Count == 1)
        {
            var touch = touches[0];

            // если палец над UI Ч не двигаем камеру
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(touch.touchId))
                return;

            Vector3 worldPos = mainCamera.ScreenToWorldPoint(touch.screenPosition);

            if (touch.phase == TouchPhase.Began)
            {
                lastWorldPos = worldPos;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 delta = lastWorldPos - worldPos;
                virtualCamera.transform.position += delta;
                lastWorldPos = worldPos;
            }
        }

        // ===== PINCH ZOOM (2 пальца) =====
        if (touches.Count == 2)
        {
            var t0 = touches[0];
            var t1 = touches[1];

            Vector2 prev0 = t0.screenPosition - t0.delta;
            Vector2 prev1 = t1.screenPosition - t1.delta;

            float prevDistance = Vector2.Distance(prev0, prev1);
            float currentDistance = Vector2.Distance(t0.screenPosition, t1.screenPosition);

            float delta = currentDistance - prevDistance;

            float size = virtualCamera.m_Lens.OrthographicSize;
            size -= delta * zoomSpeed;
            size = Mathf.Clamp(size, minZoom, maxZoom);

            virtualCamera.m_Lens.OrthographicSize = size;
        }
    }
}
