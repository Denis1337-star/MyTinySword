using UnityEngine;

/// <summary>
/// Хранит общие параметры стоимости, здоровья, времени постройки и UI-данные
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
    [Min(0)]
    [SerializeField] private int woodCost;

    [Min(0)]
    [SerializeField] private int goldCost;

    [Header("Stats")]
    [Min(1)]
    [SerializeField] private int maxHealth;

    [Header("Construction")]
    [Min(0.1f)]
    [SerializeField] private float buildTime;

    public string BuildingId => buildingId;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public GameObject BuildingPrefab => buildingPrefab;
    public int WoodCost => woodCost;
    public int GoldCost => goldCost;
    public int MaxHealth => maxHealth;
    public float BuildTime => buildTime;

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