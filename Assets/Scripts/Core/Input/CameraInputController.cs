using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class CameraInputController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0.01f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;

    private Vector2 lastTouchPos;
    private bool dragging;
    private float lastPinchDist;

    private void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
        if (Touchscreen.current == null)
            return;

        var touches = Touchscreen.current.touches;

        // ===== ZOOM (2 ןאכüצא) =====
        if (touches.Count >= 2 &&
            touches[0].isInProgress &&
            touches[1].isInProgress)
        {
            Vector2 p0 = touches[0].position.ReadValue();
            Vector2 p1 = touches[1].position.ReadValue();

            float dist = Vector2.Distance(p0, p1);

            if (lastPinchDist > 0)
            {
                float delta = dist - lastPinchDist;
                Zoom(delta);
            }

            lastPinchDist = dist;
            dragging = false;
            return;
        }
        else
        {
            lastPinchDist = 0;
        }

        // ===== MOVE (1 ןאכוצ) =====
        if (touches.Count == 1 && touches[0].isInProgress)
        {
            Vector2 pos = touches[0].position.ReadValue();

            if (!dragging)
            {
                lastTouchPos = pos;
                dragging = true;
                return;
            }

            Vector2 delta = pos - lastTouchPos;
            Move(delta);
            lastTouchPos = pos;
        }
        else
        {
            dragging = false;
        }
    }

    private void Move(Vector2 delta)
    {
        Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * moveSpeed;
        virtualCamera.transform.position += move;
    }

    private void Zoom(float delta)
    {
        float size = virtualCamera.m_Lens.OrthographicSize;
        size -= delta * zoomSpeed;
        size = Mathf.Clamp(size, minZoom, maxZoom);
        virtualCamera.m_Lens.OrthographicSize = size;
    }
}
