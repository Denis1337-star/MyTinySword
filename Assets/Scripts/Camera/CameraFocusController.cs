using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Cinemachine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CameraFocusController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CameraController cameraController;

    public bool HasFocus => virtualCamera != null && virtualCamera.Follow != null;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (cameraController == null)
            cameraController = FindObjectOfType<CameraController>(true);
    }

    private void Update()
    {
        if (cameraController != null && cameraController.IsDragging && !IsPointerOverUI())
            CancelFocus();
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
            if (TouchUtility.IsPointerOverUI(touch))
                return true;
        }

        return false;
    }
}