using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelObjectiveController : ValidatedMonoBehaviour
{
    [SerializeField] private LevelConfig _fallbackLevelConfig;

    private ResourceStorage _resourceStorage;
    private LevelRuntimeService _levelRuntimeService;
    private GameResultController _gameResultController;

    private LevelConfig _levelConfig;
    private bool _isSubscribed;

    [Inject]
    private void Construct(ResourceStorage resourceStorage,
        LevelRuntimeService levelRuntimeService,
        GameResultController gameResultController)
    {
        _resourceStorage = resourceStorage;
        _levelRuntimeService = levelRuntimeService;
        _gameResultController = gameResultController;
    }

    protected override void Awake()
    {
        base.Awake();

        if(!enabled)
            return;

        _levelConfig = ResolveLevelConfig();
        SubscribeIfNeeded();
        CheckGatherObjective();
    }
    private void OnDestroy()
    {
        Unsubscribe();
    }
    protected override bool ValidateInternal()
    {
        return true;
    }
    private LevelConfig ResolveLevelConfig()
    {
        if( _levelRuntimeService.HasCurrentLevel)
            return _levelRuntimeService.CurrentLevel;

        return _fallbackLevelConfig;
    }
    private void SubscribeIfNeeded()
    {
        if (_levelConfig == null)
            return;

        if (_levelConfig.ObjectiveType != LevelObjectiveType.GatherResource)
            return;

        _resourceStorage.ResourcesChanged += CheckGatherObjective;
        _isSubscribed = true;
    }
    private void Unsubscribe()
    {
        if (!_isSubscribed == true)
            return;

        _resourceStorage.ResourcesChanged -= CheckGatherObjective;
        _isSubscribed = false;

    }
    private void CheckGatherObjective()
    {
        if (_levelConfig == null)
            return;

        if (_levelConfig.ObjectiveType != LevelObjectiveType.GatherResource)
            return;

        int current = _resourceStorage.GetAmount(_levelConfig.GatherResourceType);

        if(current < _levelConfig.GatherTargetAmount)
            return;

        _gameResultController.FinishVictory();
    }
}
