using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// “очка на карте, на которой можно построить здание.
/// »грок кликает по ней и выбирает, что строить.
/// </summary>
public class ConstructionSlot : MonoBehaviour
{
    [Header("Available Buildings")]
    [SerializeField] private List<BuildingConfig> availableBuildings = new();

    [Header("Construction")]
    [SerializeField] private ConstructionSite constructionPrefab;

    private ConstructionSite currentConstruction;

    public IReadOnlyList<BuildingConfig> AvailableBuildings => availableBuildings;
    public bool HasConstruction => currentConstruction != null;

    /// <summary>
    /// ѕровер€ет, можно ли начать строительство на этом слоте.
    /// </summary>
    public bool CanBuild(BuildingConfig config = null)
    {
        if (currentConstruction != null)
            return false;

        if (constructionPrefab == null)
            return false;

        if (config != null && !availableBuildings.Contains(config))
            return false;

        return true;
    }

    /// <summary>
    /// «апускает строительство выбранного здани€.
    /// </summary>
    public bool StartConstruction(BuildingConfig config)
    {
        if (config == null)
            return false;

        if (!CanBuild(config))
            return false;

        if (ResourceStorage.Instance == null)
            return false;

        bool spent = ResourceStorage.Instance.TrySpendResources(config.woodCost, config.goldCost);
        if (!spent)
            return false;

        ConstructionSite site = Instantiate(constructionPrefab, transform.position, Quaternion.identity);
        site.Initialize(this, config);

        currentConstruction = site;
        gameObject.SetActive(false);

        return true;
    }

    /// <summary>
    /// ¬ызываетс€, когда строительство завершено.
    /// </summary>
    public void OnConstructionFinished()
    {
        currentConstruction = null;
        Destroy(gameObject);
    }
}