using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CameraFocusController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float focusDuration = 0.4f;

    private Coroutine focusRoutine;
    private Transform followTarget;
    private CameraController cameraController;

    public bool HasFocus => followTarget != null;

    private void Awake()
    {
        cameraController = FindObjectOfType<CameraController>();
    }

    public void FocusOn(Transform target)
    {
        if (virtualCamera == null || target == null)
            return;

        followTarget = target;

        if (focusRoutine != null)
            StopCoroutine(focusRoutine);

        focusRoutine = StartCoroutine(FocusRoutine());
    }

    public void CancelFocus()
    {
        followTarget = null;
        virtualCamera.Follow = null;
    }

    private IEnumerator FocusRoutine()
    {
        virtualCamera.Follow = followTarget;

        float timer = 0f;
        Vector3 startPos = virtualCamera.transform.position;
        Vector3 targetPos = followTarget.position;

        while (timer < focusDuration)
        {
            // ѕровер€ем, двигает ли игрок, **но не учитываем тач по UI**
            if (cameraController != null && cameraController.IsDragging() &&
                !IsPointerOverUI())
            {
                CancelFocus();
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
    private bool IsPointerOverUI()
    {
        if (Touch.activeTouches.Count == 0)
            return false;

        foreach (var touch in Touch.activeTouches)
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(touch.touchId))
                return true;
        }

        return false;
    }
}
