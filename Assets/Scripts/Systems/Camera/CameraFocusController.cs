using Cinemachine;
using UniRx;
using UnityEngine;
using Zenject;

/// <summary>
/// ”правл€ет focus режимом камеры
/// </summary>
public sealed class CameraFocusController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Transform _defaultFollowTarget;
    [SerializeField] private bool _cancelFocusOnManualDrag = true;

    private SelectionSystem _selectionSystem;
    private CompositeDisposable _disposables;

    private Transform _currentFocusTarget;
    private bool _hasFocus;

    public bool HasFocus => _hasFocus;
    public Transform CurrentFocusTarget => _currentFocusTarget;

    [Inject]
    private void Construct(SelectionSystem selectionSystem)
    {
        _selectionSystem = selectionSystem;
    }

    private void Awake()
    {
        CacheDefaultFollowTargetIfNeeded();
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
        DisposeSubscriptions();
    }

    private void OnValidate()
    {
        CacheDefaultFollowTargetIfNeeded();
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
        if (_disposables != null)
            return;

        _disposables = new CompositeDisposable();

        _selectionSystem.SelectionChanged
            .Subscribe(HandleSelectionChanged)
            .AddTo(_disposables);

        _selectionSystem.SelectionCleared
            .Subscribe(_ => CancelFocus())
            .AddTo(_disposables);

        RefreshFromCurrentSelection();
    }

    private void DisposeSubscriptions()
    {
        _disposables?.Dispose();
        _disposables = null;
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

    private Worker FindWorkerNearSelectable(UnitSelectable selectable)
    {
        if (selectable == null)
            return null;

        Worker worker = selectable.GetComponent<Worker>();

        return worker;
    }

    private void SyncDefaultTargetWithCameraPosition()
    {
        if (_defaultFollowTarget == null)
            return;

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

        _defaultFollowTarget = _virtualCamera.Follow;
    }
}