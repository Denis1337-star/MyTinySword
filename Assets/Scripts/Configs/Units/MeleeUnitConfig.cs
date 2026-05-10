using UnityEngine;

/// <summary>
/// Конфиг юнита ближнего боя
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Units/Melee Unit Config")]
public sealed class MeleeUnitConfig : UnitConfig
{
    [SerializeField, Min(1)] private int _damage;
    [SerializeField, Min(0f)] private float _attackRange;
    [SerializeField, Min(0.1f)] private float _attackCooldown;

    public override int Damage => _damage;
    public override float AttackRange => _attackRange;
    public override float AttackCooldown => _attackCooldown;

    public override bool IsValid()
    {
        return base.IsValid() &&
               UnitType == ArmyUnitType.Warrior &&
               _damage >= 1 &&
               _attackRange >= 0f &&
               _attackCooldown >= 0.1f;
    }

    public override string GetPreviewStatsText()
    {
        return
            base.GetPreviewStatsText() + "\n" +
            $"Урон: {_damage}\n" +
            $"Дистанция атаки: {_attackRange}\n" ;
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        _damage = Mathf.Max(1, _damage);
        _attackRange = Mathf.Max(0f, _attackRange);
        _attackCooldown = Mathf.Max(0.1f, _attackCooldown);
    }
}