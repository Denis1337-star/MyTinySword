using UnityEngine;

/// <summary>
/// Объект стройки.
/// Хранит прогресс строительства и создает финальное здание.
/// </summary>
public class ConstructionSite : MonoBehaviour
{
    private ConstructionSlot slot;
    private BuildingConfig config;

    private float progress;
    private float buildTime;
    private bool finished;

    public float Progress01 => buildTime > 0f ? Mathf.Clamp01(progress / buildTime) : 1f;
    public BuildingConfig Config => config;

    /// <summary>
    /// Инициализация стройки.
    /// </summary>
    public void Initialize(ConstructionSlot slot, BuildingConfig config)
    {
        this.slot = slot;
        this.config = config;

        buildTime = config != null ? Mathf.Max(0.1f, config.BuildTime) : 1f;
        progress = 0f;
        finished = false;
    }

    private void Update()
    {
        if (finished)
            return;

        progress += Time.deltaTime;

        if (progress >= buildTime)
            CompleteConstruction();
    }

    /// <summary>
    /// Завершает строительство и создаёт готовое здание.
    /// </summary>
    private void CompleteConstruction()
    {
        if (finished)
            return;

        finished = true;

        if (config == null)
        {
            Destroy(gameObject);
            return;
        }

        if (config.BuildingPrefab == null)
        {
            Destroy(gameObject);
            return;
        }

        Instantiate(config.BuildingPrefab, transform.position, Quaternion.identity);

        slot?.OnConstructionFinished();
        Destroy(gameObject);
    }
}