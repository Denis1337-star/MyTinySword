using UnityEngine;

/// <summary>
/// Хранит prefab, стоимость, боевые характеристики, данные лечения и UI-информацию
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
    [SerializeField] private int woodCost;
    [SerializeField] private int meatCost;

    [Header("Stats")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int damage;
    [SerializeField] private float moveSpeed;

    [Header("Combat")]
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float visionRange;

    [Header("Healing")]
    [SerializeField] private int healAmount;
    [SerializeField] private float healRange;
    [SerializeField] private float healCooldown;

    [Header("Projectile")]
    [SerializeField] private ProjectileArrow arrowPrefab;
    [SerializeField] private float arrowSpeed;

    [Header("Production")]
    [SerializeField] private float buildTime;

    public string UnitId => unitId;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public ArmyUnitType UnitType => unitType;
    public GameObject Prefab => prefab;
    public int WoodCost => woodCost;
    public int MeatCost => meatCost;
    public int MaxHealth => maxHealth;
    public int Damage => damage;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public float VisionRange => visionRange;
    public int HealAmount => healAmount;
    public float HealRange => healRange;
    public float HealCooldown => healCooldown;
    public ProjectileArrow ArrowPrefab => arrowPrefab;
    public float ArrowSpeed => arrowSpeed;
    public float BuildTime => buildTime;

    public override bool IsValid()
    {
        bool hasBasicInfo =
            !string.IsNullOrWhiteSpace(unitId) &&
            !string.IsNullOrWhiteSpace(displayName) &&
            prefab != null;

        bool hasBasicStats =
            maxHealth >= 1 &&
            damage >= 0 &&
            moveSpeed > 0f &&
            attackRange > 0f &&
            attackCooldown > 0f &&
            visionRange > 0f &&
            buildTime >= 0.1f &&
            arrowSpeed > 0f;

        bool hasValidHealing =
            healAmount >= 0 &&
            healRange > 0f &&
            healCooldown > 0f;

        return hasBasicInfo && hasBasicStats && hasValidHealing;
    }

    private void OnValidate()
    {
        woodCost = Mathf.Max(0, woodCost);
        meatCost = Mathf.Max(0, meatCost);

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
