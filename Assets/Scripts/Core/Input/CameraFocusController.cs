using Cinemachine;
using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private float focusBlendTime = 0.5f;

    private Transform originalFollow;
    private CinemachineBrain brain;

    private void Awake()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        originalFollow = vcam.Follow;
    }

    public void FocusOn(Transform target)
    {
        if (target == null)
            return;

        vcam.Follow = target;
        brain.m_DefaultBlend.m_Time = focusBlendTime;
    }

    public void CancelFocus()
    {
        vcam.Follow = originalFollow;
    }
}
