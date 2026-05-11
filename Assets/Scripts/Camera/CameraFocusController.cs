using Cinemachine;
using UnityEngine;
using Zenject;

/// <summary>
/// ”правл€ет focus режимом камеры
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

    public bool HasFocus => _hasFocus;
    public Transform CurrentFocusTarget => _currentFocusTarget;

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
        if (!_hasFocus)
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

    public void FocusOn(Transform target)
    {
        if (target == null)
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

    private void Subscribe()
    {
        if (_isSubscribed)
            return;

        if (_selectionSystem == null)
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

        if (_selectionSystem != null)
        {
            _selectionSystem.SelectionChanged -= HandleSelectionChanged;
            _selectionSystem.SelectionCleared -= HandleSelectionCleared;
        }

        _isSubscribed = false;
    }

    private void RefreshFromCurrentSelection()
    {
        if (_selectionSystem == null)
        {
            CancelFocus();
            return;
        }

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
        if (selectable == null)
        {
            CancelFocus();
            return;
        }

        Worker worker = FindWorkerNearSelectable(selectable);

        if (worker == null)
        {
            CancelFocus();
            return;
        }

        FocusOn(worker.transform);
    }

    private void HandleSelectionCleared()
    {
        CancelFocus();
    }

    private Worker FindWorkerNearSelectable(UnitSelectable selectable)
    {
        if (selectable == null)
            return null;

        return selectable.GetComponent<Worker>();
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