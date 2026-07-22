using TMPro;
using UnityEngine;
using Zenject;

public class LevelObjectiveHudView : ValidatedMonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private LevelConfig _fallbackLevelConfig;

    private ResourceStorage _resourceStorage;
    private LevelRuntimeService _levelRuntimeService;
    private LevelConfig _levelConfig;
    private bool _isSubscribed;

    [Inject]
    private void Construct(ResourceStorage resourceStorage,
        LevelRuntimeService levelRuntimeService)
    {
        _resourceStorage = resourceStorage;
        _levelRuntimeService = levelRuntimeService;
    }
    protected override void Awake()
    {
        base.Awake();
        if(!enabled)
            return;

        _levelConfig = ResolveLevelConfig();
        Setup();
    }
    private void OnDestroy()
    {
        if (_isSubscribed)
            _resourceStorage.ResourcesChanged -= Refresh;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;
        valid &= ValidationUtility.IsAssigned(this,_root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _progressText, nameof(_progressText));
        return valid;
    }
    private LevelConfig ResolveLevelConfig()
    {
        if(_levelRuntimeService.HasCurrentLevel)
            return _levelRuntimeService.CurrentLevel;

        return _fallbackLevelConfig;
    }
    private void Setup()
    {
        bool show = _levelConfig != null
            && _levelConfig.ObjectiveType == LevelObjectiveType.GatherResource;

        _root.SetActive(show);

        if (!show)
            return;

        _resourceStorage.ResourcesChanged += Refresh;
        _isSubscribed = true;
        Refresh();
    }
    private void Refresh()
    {
        int current = _resourceStorage.GetAmount(_levelConfig.GatherResourceType);
        int target = _levelConfig.GatherTargetAmount;

        _progressText.text = GameUiText.GatherObjectiveProgress(_levelConfig.GatherResourceType, current, target);
    }
}
