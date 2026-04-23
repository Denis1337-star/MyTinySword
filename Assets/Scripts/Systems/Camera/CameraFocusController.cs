using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Cinemachine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
///  онтроллер follow-фокуса камеры.
/// ”правл€ет тем, за каким target должна следовать Cinemachine-камера,
/// и снимает фокус, если игрок начинает вручную двигать камеру.
/// </summary>
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
    /// <summary>
    /// ѕытаетс€ восстановить отсутствующие ссылки.
    /// </summary>
    private void ResolveReferences()
    {
        if (cameraController == null)
            cameraController = FindObjectOfType<CameraController>(true);
    }

    private void Update()
    {
        // ≈сли игрок вручную двигает камеру по игровому миру,
        // считаем это €вным выходом из режима follow-фокуса
        if (cameraController != null && cameraController.IsDragging && !IsPointerOverUI())
            CancelFocus();
    }

    /// <summary>
    /// ѕереводит камеру в режим follow на указанную цель.
    /// </summary>
    public void FocusOn(Transform target)
    {
        if (virtualCamera == null || target == null)
            return;

        virtualCamera.Follow = target;
    }
    /// <summary>
    /// ѕереводит камеру в режим follow на указанную цель.
    /// </summary>
    public void CancelFocus()
    {
        if (virtualCamera == null)
            return;

        virtualCamera.Follow = null;
    }
    /// <summary>
    /// ѕровер€ет, находитс€ ли хот€ бы одно активное касание поверх UI.
    /// ≈сли да, drag по UI не должен отмен€ть фокус камеры.
    /// </summary>
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