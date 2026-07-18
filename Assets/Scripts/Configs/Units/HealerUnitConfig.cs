using UnityEngine;

/// <summary>
/// Конфиг юнита-лекаря.
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Units/Healer Unit Config")]
public sealed class HealerUnitConfig : UnitConfig
{
    [SerializeField, Min(1)] private int _healAmount;
    [SerializeField, Min(0.1f)] private float _healRange;
    [SerializeField, Min(0.1f)] private float _healCooldown;

    public override int HealAmount => _healAmount;
    public override float HealRange => _healRange;
    public override float HealCooldown => _healCooldown;

    public override bool IsValid()
    {
        return base.IsValid() &&
               UnitType == ArmyUnitType.Healer &&
               _healAmount >= 1 &&
               _healRange >= 0.1f &&
               _healCooldown >= 0.1f;
    }

    public override string GetPreviewStatsText()
    {
        return base.GetPreviewStatsText() + "\n" + GameUiText.Heal(_healAmount) + "\n";
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        _healAmount = Mathf.Max(1, _healAmount);
        _healRange = Mathf.Max(0.1f, _healRange);
        _healCooldown = Mathf.Max(0.1f, _healCooldown);
    }
}
