using UnityEngine;

/// <summary>
/// Конфиг здания
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Building Config")]
public sealed class BuildingConfig : BaseConfig
{
    [SerializeField] private string _buildingId;
    [SerializeField] private string _displayName;
    [SerializeField] private string _displayNameEn;

    [TextArea]
    [SerializeField] private string _description;

    [TextArea]
    [SerializeField] private string _descriptionEn;

    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _buildingPrefab;
    [SerializeField, Min(0)] private int _woodCost;
    [SerializeField, Min(0)] private int _goldCost;
    [SerializeField, Min(1)] private int _maxHealth;
    [SerializeField, Min(0.1f)] private float _buildTime;
    [SerializeField] private bool _uniqueBuilding;
    [SerializeField] private TechTreeBonusType _limitBonusType = TechTreeBonusType.None;

    public string BuildingId => _buildingId;
    public string DisplayName => _displayName;
    public string Description => _description;

    public string GetDisplayName(string lang) => Lang.PickDisplayName(_displayName, _displayNameEn);
    public string GetDescription(string lang) => Lang.Pick(_description, _descriptionEn);
    public Sprite Icon => _icon;
    public GameObject BuildingPrefab => _buildingPrefab;

    public int WoodCost => _woodCost;
    public int GoldCost => _goldCost;

    public int MaxHealth => _maxHealth;
    public float BuildTime => _buildTime;
    public bool UniqueBuilding => _uniqueBuilding;
    public TechTreeBonusType LimitBonusType => _limitBonusType;

    public override bool IsValid()
    {
        bool hasInfo =
            !string.IsNullOrWhiteSpace(_buildingId) &&
            !string.IsNullOrWhiteSpace(_displayName);

        bool hasPrefab = _buildingPrefab != null;

        bool hasValidNumbers =
            _woodCost >= 0 &&
            _goldCost >= 0 &&
            _maxHealth >= 1 &&
            _buildTime >= 0.1f;

        return hasInfo &&
               hasPrefab &&
               hasValidNumbers;
    }

    private void OnValidate()
    {
        _woodCost = Mathf.Max(0, _woodCost);
        _goldCost = Mathf.Max(0, _goldCost);
        _maxHealth = Mathf.Max(1, _maxHealth);
        _buildTime = Mathf.Max(0.1f, _buildTime);
    }
}