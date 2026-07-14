using UnityEngine;

/// <summary>
/// Базовый конфиг боевого юнита.
/// </summary>
public abstract class UnitConfig : BaseConfig
{
    [SerializeField] private string _unitId;
    [SerializeField] private string _displayName;
    [SerializeField] private string _displayNameEn;

    [TextArea]
    [SerializeField] private string _description;

    [TextArea]
    [SerializeField] private string _descriptionEn;

    [SerializeField] private Sprite _icon;
    [SerializeField] private ArmyUnitType _unitType;
    [SerializeField] private GameObject _prefab;
    [SerializeField, Min(0)] private int _woodCost;
    [SerializeField, Min(0)] private int _meatCost;
    [SerializeField, Min(1)] private int _maxHealth;
    [SerializeField, Min(0.1f)] private float _moveSpeed;
    [SerializeField, Min(0.1f)] private float _visionRange;
    [SerializeField, Min(0.1f)] private float _buildTime;

    public string UnitId => _unitId;
    public string DisplayName => _displayName;
    public string Description => _description;

    public string GetDisplayName(string lang) => Lang.PickDisplayName(_displayName, _displayNameEn);
    public string GetDescription(string lang) => Lang.Pick(_description, _descriptionEn);
    public Sprite Icon => _icon;
    public ArmyUnitType UnitType => _unitType;
    public GameObject Prefab => _prefab;

    public int WoodCost => _woodCost;
    public int MeatCost => _meatCost;

    public int MaxHealth => _maxHealth;
    public float MoveSpeed => _moveSpeed;
    public float VisionRange => _visionRange;

    public float BuildTime => _buildTime;

    public virtual int Damage => 0;
    public virtual float AttackRange => 0f;
    public virtual float AttackCooldown => 0f;

    public virtual int HealAmount => 0;
    public virtual float HealRange => 0f;
    public virtual float HealCooldown => 0f;

    public virtual ProjectileArrow ArrowPrefab => null;
    public virtual float ArrowSpeed => 0f;

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(_unitId) &&
               !string.IsNullOrWhiteSpace(_displayName) &&
               _prefab != null &&
               _woodCost >= 0 &&
               _meatCost >= 0 &&
               _maxHealth >= 1 &&
               _moveSpeed >= 0.1f &&
               _visionRange >= 0.1f &&
               _buildTime >= 0.1f;
    }

    public virtual string GetPreviewStatsText()
    {
        return
            $"{GameUiText.Health(_maxHealth)}\n" +
            $"{GameUiText.Speed(_moveSpeed)}\n" +
            $"{GameUiText.Vision(_visionRange)}";
    }

    protected static string FormatAttackStats(int damage, float attackRange)
    {
        return $"{GameUiText.Damage(damage)}\n{GameUiText.AttackRange(attackRange)}\n";
    }

    protected virtual void OnValidate()
    {
        _woodCost = Mathf.Max(0, _woodCost);
        _meatCost = Mathf.Max(0, _meatCost);

        _maxHealth = Mathf.Max(1, _maxHealth);
        _moveSpeed = Mathf.Max(0.1f, _moveSpeed);
        _visionRange = Mathf.Max(0.1f, _visionRange);

        _buildTime = Mathf.Max(0.1f, _buildTime);
    }
}
