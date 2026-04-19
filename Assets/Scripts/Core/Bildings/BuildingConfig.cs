using UnityEngine;

/// <summary>
/// Базовый конфиг для любого здания.
/// Хранит общие параметры стоимости, здоровья, времени постройки и UI-данные.
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Building Config")]
public class BuildingConfig : BaseConfig
{
    [Header("Info")]
    [SerializeField] private string buildingId;
    [SerializeField] private string displayName;
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    [Header("Prefab")]
    [SerializeField] private GameObject buildingPrefab;

    [Header("Cost")]
    [Min(0)] public int woodCost = 0;
    [Min(0)] public int goldCost = 0;

    [Header("Stats")]
    [Min(1)] public int maxHealth = 100;

    [Header("Construction")]
    [Min(0.1f)] public float buildTime = 5f;

    public string BuildingId => buildingId;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public GameObject BuildingPrefab => buildingPrefab;

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(buildingId) &&
               !string.IsNullOrWhiteSpace(displayName) &&
               buildingPrefab != null &&
               woodCost >= 0 &&
               goldCost >= 0 &&
               maxHealth >= 1 &&
               buildTime >= 0.1f;
    }

    private void OnValidate()
    {
        woodCost = Mathf.Max(0, woodCost);
        goldCost = Mathf.Max(0, goldCost);
        maxHealth = Mathf.Max(1, maxHealth);
        buildTime = Mathf.Max(0.1f, buildTime);
    }
}