using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float focusDuration = 0.4f;

    private Coroutine focusRoutine;
    private Transform followTarget;

    private void Awake()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("CameraFocusController: VirtualCamera НЕ назначена!");
        }
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
        if (virtualCamera == null)
            return;

        followTarget = null;
        virtualCamera.Follow = null;
    }

    private IEnumerator FocusRoutine()
    {
        virtualCamera.Follow = followTarget;
        yield return new WaitForSeconds(focusDuration);
    }
}
