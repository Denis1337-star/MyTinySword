using UnityEngine;

[CreateAssetMenu(menuName = "MyTinySword/Configs/Unit Config")]
public class UnitConfig : BaseConfig
{
    [Header("Info")]
    [SerializeField] private string unitId;
    [SerializeField] private string displayName;
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    [Header("Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Cost")]
    [Min(0)] public int woodCost = 0;
    [Min(0)] public int goldCost = 0;

    [Header("Stats")]
    [Min(1)] public int maxHealth = 100;
    [Min(0)] public int damage = 0;
    [Min(0.1f)] public float moveSpeed = 3f;

    [Header("Production")]
    [Min(0.1f)] public float buildTime = 3f;

    public string UnitId => unitId;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public GameObject Prefab => prefab;

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(unitId) &&
               !string.IsNullOrWhiteSpace(displayName) &&
               prefab != null &&
               maxHealth >= 1 &&
               damage >= 0 &&
               buildTime >= 0.1f;
    }

    private void OnValidate()
    {
        woodCost = Mathf.Max(0, woodCost);
        goldCost = Mathf.Max(0, goldCost);
        maxHealth = Mathf.Max(1, maxHealth);
        damage = Mathf.Max(0, damage);
        moveSpeed = Mathf.Max(0.1f, moveSpeed);
        buildTime = Mathf.Max(0.1f, buildTime);
    }
}
