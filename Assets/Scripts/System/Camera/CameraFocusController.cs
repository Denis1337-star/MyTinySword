using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CameraFocusController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CameraController cameraController;

    public bool HasFocus => virtualCamera != null && virtualCamera.Follow != null;

    private void Awake()
    {
        if (cameraController == null)
            cameraController = FindObjectOfType<CameraController>();
    }

    private void Update()
    {
        if (cameraController != null && cameraController.IsDragging && !IsPointerOverUI())
        {
            CancelFocus();
        }
    }

    public void FocusOn(Transform target)
    {
        if (virtualCamera == null || target == null)
            return;

        virtualCamera.Follow = target;
    }

    public void CancelFocus()
    {
        if (virtualCamera == null)
            return;

        virtualCamera.Follow = null;
    }

    private bool IsPointerOverUI()
    {
        if (Touch.activeTouches.Count == 0)
            return false;

        foreach (var touch in Touch.activeTouches)
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(touch.touchId))
            {
                return true;
            }
        }

        return false;
    }
}
