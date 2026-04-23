using UnityEngine;

/// <summary>
/// Конфиг юнита.
/// Хранит prefab, стоимость, время найма и UI-данные.
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Unit Config")]
public class UnitConfig : BaseConfig
{
    [Header("Info")]
    [SerializeField] private string unitId;
    [SerializeField] private string displayName;
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private ArmyUnitType unitType;

    [Header("Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Cost")]
    [Min(0)] public int woodCost = 0;
    [Min(0)] public int goldCost = 0;

    [Header("Stats")]
    [Min(1)] public int maxHealth = 100;
    [Min(0)] public int damage = 0;
    [Min(0.1f)] public float moveSpeed = 3f;

    [Header("Combat")]
    [Min(0.1f)] public float attackRange = 1.5f;
    [Min(0.1f)] public float attackCooldown = 1f;
    [Min(0.1f)] public float visionRange = 4f;

    [Header("Healing")]
    [Min(0)] public int healAmount = 0;
    [Min(0.1f)] public float healRange = 3f;
    [Min(0.1f)] public float healCooldown = 1.2f;

    [Header("Projectile")]
    [SerializeField] private ProjectileArrow arrowPrefab;
    [Min(0.1f)] public float arrowSpeed = 8f;

    [Header("Production")]
    [Min(0.1f)] public float buildTime = 3f;

    public string UnitId => unitId;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public ArmyUnitType UnitType => unitType;
    public GameObject Prefab => prefab;
    public ProjectileArrow ArrowPrefab => arrowPrefab;

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(unitId) &&
               !string.IsNullOrWhiteSpace(displayName) &&
               prefab != null &&
               maxHealth >= 1 &&
               damage >= 0 &&
               moveSpeed > 0f &&
               attackRange > 0f &&
               attackCooldown > 0f &&
               visionRange > 0f &&
               healRange > 0f &&
               healCooldown > 0f &&
               buildTime >= 0.1f;
    }

    private void OnValidate()
    {
        woodCost = Mathf.Max(0, woodCost);
        goldCost = Mathf.Max(0, goldCost);

        maxHealth = Mathf.Max(1, maxHealth);
        damage = Mathf.Max(0, damage);
        moveSpeed = Mathf.Max(0.1f, moveSpeed);

        attackRange = Mathf.Max(0.1f, attackRange);
        attackCooldown = Mathf.Max(0.1f, attackCooldown);
        visionRange = Mathf.Max(0.1f, visionRange);

        healAmount = Mathf.Max(0, healAmount);
        healRange = Mathf.Max(0.1f, healRange);
        healCooldown = Mathf.Max(0.1f, healCooldown);

        arrowSpeed = Mathf.Max(0.1f, arrowSpeed);
        buildTime = Mathf.Max(0.1f, buildTime);
    }
}
