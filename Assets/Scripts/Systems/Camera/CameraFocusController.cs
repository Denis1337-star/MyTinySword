using Cinemachine;
using UnityEngine;

/// <summary>
/// Управляет focus-режимом Cinemachine камеры
/// Может сфокусировать камеру на цели и отменяет focus при ручном drag камеры
/// </summary>
public class CameraFocusController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CameraController _cameraController;

    private Transform _currentTarget;

    public bool HasFocus => _currentTarget != null;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        if (!HasFocus)
            return;

        if (_cameraController == null)
            return;

        if (_cameraController.IsDragging)
            CancelFocus();
    }

    /// <summary>
    /// Переводит камеру в follow-режим на указанную цель
    /// </summary>
    public void FocusOn(Transform target)
    {
        if (target == null || _virtualCamera == null)
            return;

        _currentTarget = target;
        _virtualCamera.Follow = target;
    }

    /// <summary>
    /// Отменяет follow-режим камеры
    /// </summary>
    public void CancelFocus()
    {
        _currentTarget = null;

        if (_virtualCamera != null)
            _virtualCamera.Follow = null;
    }

    private void ResolveReferences()
    {
        if (_virtualCamera == null)
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (_cameraController == null)
            _cameraController = GetComponent<CameraController>();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }
}