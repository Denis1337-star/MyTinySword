using UnityEngine;

/// <summary>
/// Объект стройки
/// </summary>
public sealed class ConstructionSite : MonoBehaviour
{
    private ConstructionSlot _slot;
    private BuildingConfig _config;
    private BuildingRegistry _buildingRegistry;
    private BuildingFactory _buildingFactory;

    private float _progress;
    private float _buildTime;
    private bool _initialized;
    private bool _finished;
    private bool _completionAttempted;

    public float Progress01 => _buildTime > 0f ? Mathf.Clamp01(_progress / _buildTime) : 1f;
    public BuildingConfig Config => _config;

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

        _buildTime = config.BuildTime;
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
            _config.BuildingPrefab,transform.position, Quaternion.identity);

        if (building == null)
        {
            Debug.LogError(
                $"{name}: не удалось создать здание из BuildingConfig \"{_config.name}\".",
                this);

            return;
        }

        AttachSlotToBuilding(building);

        _finished = true;

        _buildingRegistry?.RegisterBuilt(_config);

        NotifySlotAndDestroy();
    }

    private void AttachSlotToBuilding(GameObject building)
    {
        if (building == null)
            return;

        BuildingBase buildingBase = building.GetComponent<BuildingBase>();

        if (buildingBase == null)
        {
            Debug.LogError(
                $"{name}: созданное здание \"{building.name}\" не имеет BuildingBase. " +
                "Слот не сможет вернуться после сноса здания.",
                building);

            return;
        }

        buildingBase.AttachConstructionSlot(_slot);
    }

    private void NotifySlotAndDestroy()
    {
        _slot?.OnConstructionFinished();
        Destroy(gameObject);
    }
}