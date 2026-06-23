using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Каталог всех нод дерева развития.
/// Используется UI и gameplay-сервисами как единый источник config-нод.
/// </summary>
[CreateAssetMenu(
    fileName = "TechTreeCatalogConfig",
    menuName = "MyTinySword/Tech Tree/Catalog Config")]
public sealed class TechTreeCatalogConfig : BaseConfig
{
    [Header("Nodes")]
    [SerializeField] private List<TechTreeNodeConfig> _nodes = new();

    public IReadOnlyList<TechTreeNodeConfig> Nodes => _nodes;

    public TechTreeNodeConfig GetByBonusType(TechTreeBonusType bonusType)
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            TechTreeNodeConfig node = _nodes[i];

            if (node != null && node.BonusType == bonusType)
                return node;
        }

        return null;
    }

    public override bool IsValid()
    {
        bool valid = true;

        if (_nodes == null || _nodes.Count == 0)
        {
            Debug.LogError($"{name}: список нод дерева развития пуст.", this);
            return false;
        }

        HashSet<string> nodeIds = new();
        HashSet<TechTreeBonusType> bonusTypes = new();

        for (int i = 0; i < _nodes.Count; i++)
        {
            TechTreeNodeConfig node = _nodes[i];

            if (node == null)
            {
                Debug.LogError($"{name}: Node Config с индексом {i} не назначен.", this);
                valid = false;
                continue;
            }

            valid &= node.IsValid();

            if (!nodeIds.Add(node.NodeId))
            {
                Debug.LogError($"{name}: повторяется Node Id: {node.NodeId}.", this);
                valid = false;
            }

            if (node.BonusType != TechTreeBonusType.None && !bonusTypes.Add(node.BonusType))
            {
                Debug.LogError($"{name}: повторяется Bonus Type: {node.BonusType}.", this);
                valid = false;
            }
        }

        return valid;
    }
}