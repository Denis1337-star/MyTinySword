using System;
using UnityEngine;

/// <summary>
/// Конфиг одной ноды дерева развития
/// </summary>
[CreateAssetMenu(
    fileName = "TechTreeNodeConfig",
    menuName = "MyTinySword/Tech Tree/Node Config")]
public sealed class TechTreeNodeConfig : BaseConfig
{
    [Header("Identity")]
    [SerializeField] private string _nodeId = "node_id";

    [Header("View")]
    [SerializeField] private string _displayName = "Новая нода";
    [SerializeField] private string _displayNameEn = "New node";
    [SerializeField, TextArea] private string _description = "Описание ноды.";
    [SerializeField, TextArea] private string _descriptionEn = "Node description.";
    [SerializeField] private Sprite _icon;

    [Header("Progress")]
    [SerializeField, Min(1)] private int _maxLevel = 3;
    [SerializeField] private int[] _upgradeSeconds = { 30, 60, 120 };

    [Header("Bonus")]
    [SerializeField] private TechTreeBonusType _bonusType = TechTreeBonusType.None;
    [SerializeField] private float _bonusPerLevel = 1f;
    [SerializeField] private string _bonusPrefix = "+";
    [SerializeField] private string _bonusSuffix = "%";

    [Header("Requirements")]
    [SerializeField] private TechTreeRequirement[] _requirements;

    public string NodeId => _nodeId;
    public string DisplayName => _displayName;
    public string Description => _description;

    public string GetDisplayName(string lang) => Lang.PickDisplayName(_displayName, _displayNameEn);
    public string GetDescription(string lang) => Lang.Pick(_description, _descriptionEn);
    public Sprite Icon => _icon;
    public int MaxLevel => _maxLevel;
    public TechTreeBonusType BonusType => _bonusType;
    public float BonusPerLevel => _bonusPerLevel;
    public string BonusPrefix => _bonusPrefix;
    public string BonusSuffix => _bonusSuffix;
    public TechTreeRequirement[] Requirements => _requirements ?? Array.Empty<TechTreeRequirement>();

    public int GetUpgradeSeconds(int currentLevel)
    {
        if (_upgradeSeconds == null || currentLevel < 0 || currentLevel >= _upgradeSeconds.Length)
            return 0;

        return _upgradeSeconds[currentLevel];
    }

    public string GetBonusText(int level)
    {
        float value = _bonusPerLevel * level;

        if (Mathf.Approximately(value % 1f, 0f))
            return $"{_bonusPrefix}{(int)value}{_bonusSuffix}";

        return $"{_bonusPrefix}{value:0.#}{_bonusSuffix}";
    }

    public override bool IsValid()
    {
        bool valid = true;

        if (string.IsNullOrWhiteSpace(_nodeId))
        {
            Debug.LogError($"{name}: Node Id не задан.", this);
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(_displayName))
        {
            Debug.LogError($"{name}: Display Name не задан.", this);
            valid = false;
        }

        if (_maxLevel < 1)
        {
            Debug.LogError($"{name}: Max Level должен быть больше 0.", this);
            valid = false;
        }

        if (_upgradeSeconds == null || _upgradeSeconds.Length < _maxLevel)
        {
            Debug.LogError($"{name}: Upgrade Seconds должен содержать время для каждого уровня.", this);
            valid = false;
        }

        if (_requirements != null)
        {
            for (int i = 0; i < _requirements.Length; i++)
            {
                TechTreeRequirement requirement = _requirements[i];

                if (requirement == null)
                {
                    Debug.LogError($"{name}: Requirement {i} не настроен.", this);
                    valid = false;
                    continue;
                }

                if (requirement.RequiredNode == null)
                {
                    Debug.LogError($"{name}: Required Node в Requirement {i} не задан.", this);
                    valid = false;
                }

                if (requirement.RequiredLevel < 1)
                {
                    Debug.LogError($"{name}: Required Level в Requirement {i} должен быть больше 0.", this);
                    valid = false;
                }
            }
        }

        return valid;
    }
}