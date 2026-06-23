using UnityEngine;
using Zenject;

/// <summary>
/// Объект стройки
/// </summary>
public sealed class ConstructionSite : MonoBehaviour
{
    private ConstructionSlot _slot;
    private BuildingConfig _config;
    private BuildingRegistry _buildingRegistry;
    private BuildingFactory _buildingFactory;
    private GameAudioService _audioService;
    private TechTreeBonusService _techTreeBonusService;

    private float _progress;
    private float _buildTime;
    private bool _initialized;
    private bool _finished;
    private bool _completionAttempted;

    public float Progress01 => _buildTime > 0f ? Mathf.Clamp01(_progress / _buildTime) : 1f;
    public BuildingConfig Config => _config;

    [Inject]
    private void Construct(
        GameAudioService audioService,
        TechTreeBonusService techTreeBonusService)
    {
        _audioService = audioService;
        _techTreeBonusService = techTreeBonusService;
    }

    public void Initialize(
        ConstructionSlot slot,
        BuildingConfig config,
        BuildingRegistry buildingRegistry,
        BuildingFactory buildingFactory)
    {
        _slot = slot;
        _config = config;
        _buildingRegistry = buildingRegistry;
        _buildingFactory = buildingFactory;

        _buildTime = _techTreeBonusService.ApplyPercentReduction(
         config.BuildTime,
         TechTreeBonusType.BuildAll);

        _buildTime = Mathf.Max(0.1f, _buildTime);

        _progress = 0f;
        _finished = false;
        _completionAttempted = false;
        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized || _finished || _completionAttempted)
            return;

        _progress += Time.deltaTime;

        if (_progress >= _buildTime)
            CompleteConstruction();
    }

    private void OnDestroy()
    {
        if (!_initialized)
            return;

        if (_finished)
            return;

        _buildingRegistry?.UnregisterConstruction(_config);
    }

    private void CompleteConstruction()
    {
        if (_finished || _completionAttempted)
            return;

        _completionAttempted = true;

        GameObject building = _buildingFactory.CreateBuilding(
            _config.BuildingPrefab,
            transform.position,
            Quaternion.identity);

        if (building == null)
        {
            Debug.LogError(
                $"{name}: не удалось создать здание из BuildingConfig \"{_config.name}\".",
                this);

            return;
        }

        BuildingBase buildingBase = building.GetComponent<BuildingBase>();

        if (buildingBase == null)
        {
            Debug.LogError($"{name}: prefab не содержит BuildingBase.", building);
            return;
        }

        AttachSlotToBuilding(buildingBase);
        PlayBuiltSound();

        _finished = true;

        _buildingRegistry?.RegisterBuilt(_config, buildingBase);

        NotifySlotAndDestroy();
    }

    private void AttachSlotToBuilding(BuildingBase buildingBase)
    {
        buildingBase.AttachConstructionSlot(_slot);
    }

    private void PlayBuiltSound()
    {
        _audioService.PlayWorldSound(SoundId.BuildingBuilt, transform.position);
    }

    private void NotifySlotAndDestroy()
    {
        _slot?.OnConstructionFinished();
        Destroy(gameObject);
    }
}