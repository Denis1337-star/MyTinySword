using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float focusDuration = 0.4f;

    private Coroutine focusRoutine;
    private Transform followTarget;

    public void FocusOn(Transform target)
    {
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
        yield return new WaitForSeconds(focusDuration);
    }
}
