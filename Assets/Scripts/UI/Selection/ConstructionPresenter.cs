using UniRx;
using UnityEngine;
using Zenject;

/// <summary>
/// ”правл€ет отображением панели строительства дл€  ConstructionSlot
/// </summary>
public sealed class ConstructionPresenter : MonoBehaviour
{
    [SerializeField] private ConstructionPanel _constructionPanel;

    private SelectionSystem _selectionSystem;
    private CompositeDisposable _disposables;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        ConstructionPanel constructionPanel)
    {
        _selectionSystem = selectionSystem;
        _constructionPanel = constructionPanel;
    }

    private void Awake()
    {
        _constructionPanel?.Hide();
    }

    private void Start()
    {
        Subscribe();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        DisposeSubscriptions();
    }

    private void HandleSelectionChanged(UnitSelectable selectable)
    {
        if (_constructionPanel == null)
            return;

        if (selectable == null)
        {
            _constructionPanel.Hide();
            return;
        }

        ConstructionSlot slot = FindComponentNearSelectable<ConstructionSlot>(selectable);

        if (slot == null)
        {
            _constructionPanel.Hide();
            return;
        }

        _constructionPanel.Show(slot);
    }

    private void HandleSelectionCleared(Unit _)
    {
        _constructionPanel?.Hide();
    }

    private void Subscribe()
    {
        if (_selectionSystem == null)
            return;

        if (_disposables != null)
            return;

        _disposables = new CompositeDisposable();

        _selectionSystem.SelectionChanged
            .Subscribe(HandleSelectionChanged)
            .AddTo(_disposables);

        _selectionSystem.SelectionCleared
            .Subscribe(HandleSelectionCleared)
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
        if (_selectionSystem == null)
        {
            _constructionPanel?.Hide();
            return;
        }

        UnitSelectable currentSelection = _selectionSystem.CurrentSelection;

        if (currentSelection == null)
        {
            HandleSelectionCleared(Unit.Default);
            return;
        }

        HandleSelectionChanged(currentSelection);
    }

    private T FindComponentNearSelectable<T>(UnitSelectable selectable) where T : Component
    {
        if (selectable == null)
            return null;

        T component = selectable.GetComponent<T>();

        if (component != null)
            return component;

        component = selectable.GetComponentInParent<T>();

        if (component != null)
            return component;

        return selectable.GetComponentInChildren<T>();
    }
}