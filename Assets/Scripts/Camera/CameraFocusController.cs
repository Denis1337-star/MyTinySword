using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using Zenject;

/// <summary>
/// Единая точка управления Cinemachine: follow рабочего и cinematic tutorial.
/// </summary>
public sealed class CameraFocusController : ValidatedMonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Transform _defaultFollowTarget;
    [SerializeField] private bool _cancelFocusOnManualDrag = true;

    private SelectionSystem _selectionSystem;

    private Transform _currentFocusTarget;
    private bool _hasFocus;
    private bool _isSubscribed;
    private bool _tutorialControlsCamera;
    private Coroutine _tutorialRoutine;

    public bool HasFocus => _hasFocus;
    public Transform CurrentFocusTarget => _currentFocusTarget;
    public bool IsTutorialFollowActive => _tutorialRoutine != null;

    [Inject]
    private void Construct(SelectionSystem selectionSystem)
    {
        _selectionSystem = selectionSystem;
    }

    protected override void Awake()
    {
        CacheDefaultFollowTargetIfNeeded();

        base.Awake();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void Update()
    {
        if (_tutorialControlsCamera || !_hasFocus)
            return;

        if (!_cancelFocusOnManualDrag)
            return;

        if (!_cameraController.IsDragging)
            return;

        CancelFocus();
    }

    private void OnDisable()
    {
        Unsubscribe();
        StopTutorialCamera();
    }

    private void OnValidate()
    {
        CacheDefaultFollowTargetIfNeeded();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _virtualCamera, nameof(_virtualCamera));
        valid &= ValidationUtility.IsAssigned(this, _cameraController, nameof(_cameraController));
        valid &= ValidationUtility.IsAssigned(this, _defaultFollowTarget, nameof(_defaultFollowTarget));

        return valid;
    }

    public void BeginTutorialCamera()
    {
        _tutorialControlsCamera = true;
        _cameraController.SetInputEnabled(false);
        CancelFocus();
    }

    public void EndTutorialCamera()
    {
        StopTutorialCamera();
        _tutorialControlsCamera = false;
        _cameraController.SetInputEnabled(true);
        _virtualCamera.Follow = _defaultFollowTarget;
    }

    public void TutorialFocusOn(Transform target, Action onComplete = null)
    {
        if (!_tutorialControlsCamera || target == null)
        {
            onComplete?.Invoke();
            return;
        }

        StopTutorialRoutine();
        _virtualCamera.Follow = target;
        onComplete?.Invoke();
    }

    public void TutorialFollow(Transform target, Func<bool> shouldStop)
    {
        if (!_tutorialControlsCamera || target == null)
            return;

        StopTutorialRoutine();
        _tutorialRoutine = StartCoroutine(TutorialFollowRoutine(target, shouldStop));
    }

    public void StopTutorialCamera()
    {
        StopTutorialRoutine();

        if (!_tutorialControlsCamera)
            return;

        _virtualCamera.Follow = _defaultFollowTarget;
    }

    public void FocusOn(Transform target)
    {
        if (target == null || _tutorialControlsCamera)
            return;

        _currentFocusTarget = target;
        _hasFocus = true;

        _virtualCamera.Follow = target;
    }

    public void CancelFocus()
    {
        if (!_hasFocus)
            return;

        SyncDefaultTargetWithCameraPosition();

        _currentFocusTarget = null;
        _hasFocus = false;

        _virtualCamera.Follow = _defaultFollowTarget;
    }

    private IEnumerator TutorialFollowRoutine(Transform target, Func<bool> shouldStop)
    {
        _virtualCamera.Follow = target;

        while (target != null && shouldStop != null && !shouldStop())
            yield return null;

        _tutorialRoutine = null;
    }

    private void StopTutorialRoutine()
    {
        if (_tutorialRoutine == null)
            return;

        StopCoroutine(_tutorialRoutine);
        _tutorialRoutine = null;
    }

    private void Subscribe()
    {
        if (_isSubscribed)
            return;

        _selectionSystem.SelectionChanged += HandleSelectionChanged;
        _selectionSystem.SelectionCleared += HandleSelectionCleared;

        _isSubscribed = true;

        RefreshFromCurrentSelection();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
            return;

        _selectionSystem.SelectionChanged -= HandleSelectionChanged;
        _selectionSystem.SelectionCleared -= HandleSelectionCleared;

        _isSubscribed = false;
    }

    private void RefreshFromCurrentSelection()
    {
        UnitSelectable currentSelection = _selectionSystem.CurrentSelection;

        if (currentSelection == null)
        {
            CancelFocus();
            return;
        }

        HandleSelectionChanged(currentSelection);
    }

    private void HandleSelectionChanged(UnitSelectable selectable)
    {
        if (_tutorialControlsCamera)
            return;

        if (selectable == null)
        {
            CancelFocus();
            return;
        }

        Worker worker = SelectableUtility.FindNear<Worker>(selectable);

        if (worker == null)
        {
            CancelFocus();
            return;
        }

        FocusOn(worker.transform);
    }

    private void HandleSelectionCleared()
    {
        if (_tutorialControlsCamera)
            return;

        CancelFocus();
    }

    private void SyncDefaultTargetWithCameraPosition()
    {
        Vector3 cameraPosition = _virtualCamera.transform.position;

        _defaultFollowTarget.position = new Vector3(
            cameraPosition.x,
            cameraPosition.y,
            _defaultFollowTarget.position.z);
    }

    private void CacheDefaultFollowTargetIfNeeded()
    {
        if (_defaultFollowTarget != null)
            return;

        if (_virtualCamera == null)
            return;

        _defaultFollowTarget = _virtualCamera.Follow;
    }
}
