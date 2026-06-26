using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI линия между двумя нодами дерева развития.
/// Цвет показывает состояние дочерней ноды.
/// </summary>
public sealed class TechTreeConnectionView : ValidatedMonoBehaviour
{
    [Header("Connection")]
    [SerializeField] private TechTreeNodeConfig _fromNode;
    [SerializeField] private TechTreeNodeConfig _toNode;

    [Header("Visual")]
    [SerializeField] private Image _lineImage;

    [Header("Colors")]
    [SerializeField] private Color _lockedColor = new(0.35f, 0.35f, 0.35f, 1f);
    [SerializeField] private Color _availableColor = new(1f, 0.85f, 0.25f, 1f);
    [SerializeField] private Color _upgradingColor = new(0.35f, 0.75f, 1f, 1f);
    [SerializeField] private Color _maxedColor = new(0.35f, 1f, 0.35f, 1f);

    public TechTreeNodeConfig FromNode => _fromNode;
    public TechTreeNodeConfig ToNode => _toNode;

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsValidConfig(this, _fromNode, nameof(_fromNode));
        valid &= ValidationUtility.IsValidConfig(this, _toNode, nameof(_toNode));
        valid &= ValidationUtility.IsAssigned(this, _lineImage, nameof(_lineImage));

        return valid;
    }

    public void Refresh(TechTreeNodeState toNodeState)
    {
        _lineImage.color = toNodeState switch
        {
            TechTreeNodeState.Locked => _lockedColor,
            TechTreeNodeState.Upgrading => _upgradingColor,
            TechTreeNodeState.Maxed => _maxedColor,
            _ => _availableColor
        };
    }
}