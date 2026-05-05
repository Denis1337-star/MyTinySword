using UnityEngine;

/// <summary>
/// Конфиг юнита дальнего боя
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Units/Ranged Unit Config")]
public sealed class RangedUnitConfig : UnitConfig
{
    [SerializeField, Min(1)] private int _damage;
    [SerializeField, Min(0.1f)] private float _attackRange;
    [SerializeField, Min(0.1f)] private float _attackCooldown;
    [SerializeField] private ProjectileArrow _arrowPrefab;
    [SerializeField, Min(0.1f)] private float _arrowSpeed;

    public override int Damage => _damage;
    public override float AttackRange => _attackRange;
    public override float AttackCooldown => _attackCooldown;

    public override ProjectileArrow ArrowPrefab => _arrowPrefab;
    public override float ArrowSpeed => _arrowSpeed;

    public override bool IsValid()
    {
        return base.IsValid() &&
               _damage >= 1 &&
               _attackRange >= 0.1f &&
               _attackCooldown >= 0.1f &&
               _arrowPrefab != null &&
               _arrowSpeed >= 0.1f;
    }

    public override string GetPreviewStatsText()
    {
        return
            base.GetPreviewStatsText() + "\n" +
            $"Damage: {_damage}\n" +
            $"Attack Range: {_attackRange}\n" +
            $"Attack Cooldown: {_attackCooldown}\n" +
            $"Arrow Speed: {_arrowSpeed}";
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        _damage = Mathf.Max(1, _damage);
        _attackRange = Mathf.Max(0.1f, _attackRange);
        _attackCooldown = Mathf.Max(0.1f, _attackCooldown);
        _arrowSpeed = Mathf.Max(0.1f, _arrowSpeed);
    }
}