using System.Text;
using UnityEngine;
using YG;
using Zenject;

/// <summary>
/// Главный контроллер UI дерева развития.
/// Связывает ноды, линии, InfoPanel и сохранение прогресса.
/// </summary>
public sealed class TechTreePanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;
    [SerializeField] private SimplePanelTween _panelTween;

    [Header("Views")]
    [SerializeField] private TechTreeInfoPanel _infoPanel;
    [SerializeField] private TechTreeMapPanController _mapPanController;
    [SerializeField] private TechTreeNodeView[] _nodeViews;
    [SerializeField] private TechTreeConnectionView[] _connectionViews;

    [Header("Update")]
    [SerializeField, Min(0.05f)] private float _refreshInterval = 0.25f;

    private readonly StringBuilder _stringBuilder = new();

    private TechTreeSaveService _saveService;
    private TechTreeNodeView _selectedNodeView;
    private TechTreeCatalogConfig _catalog;

    private float _nextRefreshTime;

    [Inject]
    private void Construct(
    TechTreeSaveService saveService,
    TechTreeCatalogConfig catalog)
    {
        _saveService = saveService;
        _catalog = catalog;
    }


    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _infoPanel, nameof(_infoPanel));
        valid &= ValidationUtility.IsAssigned(this, _mapPanController, nameof(_mapPanController));
        valid &= ValidationUtility.ValidArray(this, _nodeViews, nameof(_nodeViews));
        valid &= ValidationUtility.ValidArray(this, _connectionViews, nameof(_connectionViews));

        return valid;
    }

    private void OnEnable()
    {
        YG2.onSwitchLang += HandleLanguageSwitched;
        Subscribe();
        RefreshAll();
        _infoPanel.HideImmediate();
    }

    private void OnDisable()
    {
        YG2.onSwitchLang -= HandleLanguageSwitched;
        Unsubscribe();
    }

    private void HandleLanguageSwitched(string lang)
    {
        RefreshAll();
    }

    private void Update()
    {
        if (Time.unscaledTime < _nextRefreshTime)
            return;

        _nextRefreshTime = Time.unscaledTime + _refreshInterval;

        RefreshAll();
    }

    public void Show()
    {
        RefreshAll();

        if (_panelTween != null)
        {
            _panelTween.Show();
            return;
        }

        _root.SetActive(true);
    }

    public void Hide()
    {
        ClearSelection();

        if (_panelTween != null)
        {
            _panelTween.Hide();
            return;
        }

        _root.SetActive(false);
    }

    public void Toggle()
    {
        if (_panelTween != null && _panelTween.IsVisible)
        {
            Hide();
            return;
        }

        if (_root.activeSelf)
        {
            Hide();
            return;
        }

        Show();
    }

    private void Subscribe()
    {
        for (int i = 0; i < _nodeViews.Length; i++)
            _nodeViews[i].Initialize(HandleNodeClicked);

        _infoPanel.UpgradeClicked += HandleUpgradeClicked;
        _mapPanController.DragStarted += HandleMapDragStarted;
    }

    private void Unsubscribe()
    {
        _infoPanel.UpgradeClicked -= HandleUpgradeClicked;
        _mapPanController.DragStarted -= HandleMapDragStarted;
    }

    private void HandleNodeClicked(TechTreeNodeView nodeView)
    {
        _selectedNodeView = nodeView;

        RefreshAll();
        RefreshInfoPanel();
        _infoPanel.Show();
    }

    private void HandleUpgradeClicked()
    {
        if (_selectedNodeView == null)
            return;

        bool started = _saveService.TryStartUpgrade(_selectedNodeView.Config);

        if (!started)
            return;

        RefreshAll();
        RefreshInfoPanel();
    }

    private void HandleMapDragStarted()
    {
        ClearSelection();
    }

    private void ClearSelection()
    {
        _selectedNodeView = null;
        _infoPanel.Hide();
        RefreshAll();
    }

    private void RefreshAll()
    {
        CompleteReadyUpgrades();
        RefreshNodes();
        RefreshConnections();

        if (_selectedNodeView != null)
            RefreshInfoPanel();
    }

    private void CompleteReadyUpgrades()
    {
        _saveService.CompleteReadyUpgrades(_catalog.Nodes);
    }

    private void RefreshNodes()
    {
        for (int i = 0; i < _nodeViews.Length; i++)
        {
            TechTreeNodeView nodeView = _nodeViews[i];
            TechTreeNodeConfig config = nodeView.Config;
            TechTreeNodeSaveData saveData = _saveService.GetOrCreateNode(config);
            TechTreeNodeState state = GetVisualState(nodeView);
            long remainingSeconds = _saveService.GetRemainingSeconds(config);
            bool selected = nodeView == _selectedNodeView;

            nodeView.Refresh(saveData, state, selected, remainingSeconds);
        }
    }

    private void RefreshConnections()
    {
        for (int i = 0; i < _connectionViews.Length; i++)
        {
            TechTreeConnectionView connectionView = _connectionViews[i];
            TechTreeNodeState toNodeState = _saveService.GetNodeState(connectionView.ToNode);
            connectionView.Refresh(toNodeState);
        }
    }

    private void RefreshInfoPanel()
    {
        if (_selectedNodeView == null)
            return;

        TechTreeNodeConfig config = _selectedNodeView.Config;
        TechTreeNodeSaveData saveData = _saveService.GetOrCreateNode(config);
        TechTreeNodeState state = GetVisualState(_selectedNodeView);
        long remainingSeconds = _saveService.GetRemainingSeconds(config);
        string requirementsText = BuildRequirementsText(config);

        bool anotherNodeUpgrading =
    _saveService.HasAnyActiveUpgrade() &&
    state != TechTreeNodeState.Upgrading;

        _infoPanel.Refresh(
            config,
            saveData,
            state,
            requirementsText,
            remainingSeconds,
            anotherNodeUpgrading);

    }

    private TechTreeNodeState GetVisualState(TechTreeNodeView nodeView)
    {
        TechTreeNodeState state = _saveService.GetNodeState(nodeView.Config);

        if (nodeView == _selectedNodeView && state == TechTreeNodeState.Available)
            return TechTreeNodeState.Selected;

        return state;
    }

    private string BuildRequirementsText(TechTreeNodeConfig config)
    {
        TechTreeRequirement[] requirements = config.Requirements;

        if (requirements.Length == 0)
            return GameUiText.NoRequirements;

        _stringBuilder.Clear();

        for (int i = 0; i < requirements.Length; i++)
        {
            TechTreeRequirement requirement = requirements[i];
            TechTreeNodeSaveData requiredSaveData = _saveService.GetOrCreateNode(requirement.RequiredNode);

            bool completed = requiredSaveData.Level >= requirement.RequiredLevel;

            _stringBuilder.Append(completed ? "✓ " : "• ");
            _stringBuilder.Append(requirement.RequiredNode.GetDisplayName(Lang.Current));
            _stringBuilder.Append(": ");
            _stringBuilder.Append(requiredSaveData.Level);
            _stringBuilder.Append("/");
            _stringBuilder.Append(requirement.RequiredLevel);

            if (i < requirements.Length - 1)
                _stringBuilder.AppendLine();
        }

        return _stringBuilder.ToString();
    }
}