using UnityEngine;

/// <summary>
/// Объект стройки
/// </summary>
public class ConstructionSite : MonoBehaviour
{
    private ConstructionSlot _slot;
    private BuildingConfig _config;
    private BuildingRegistry _buildingRegistry;
    private BuildingFactory _buildingFactory;

    private float _progress;
    private float _buildTime;
    private bool _initialized;
    private bool _finished;

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
        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized || _finished)
            return;

        _progress += Time.deltaTime;

        if (_progress >= _buildTime)
            CompleteConstruction();
    }

    private void CompleteConstruction()
    {
        if (_finished)
            return;

        _finished = true;

        if (_buildingFactory == null)
        {
            Debug.LogError($"{name}: BuildingFactory не передан в ConstructionSite.", this);
            return;
        }

        GameObject building = _buildingFactory.CreateBuilding(
            _config.BuildingPrefab,
            transform.position,
            Quaternion.identity);

        if (building == null)
            return;

        _buildingRegistry?.RegisterBuilt(_config);

        NotifySlotAndDestroy();
    }

    private void NotifySlotAndDestroy()
    {
        _slot?.OnConstructionFinished();
        Destroy(gameObject);
    }
}