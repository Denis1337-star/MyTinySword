using UnityEngine;

/// <summary>
/// Выполняет боевые действия юнита
/// </summary>
public sealed class ArmyUnitCombat
{
    private readonly ArmyUnit _unit;
    private readonly Transform _origin;
    private readonly GameAudioService _audioService;


    public ArmyUnitCombat( ArmyUnit unit,
        Transform origin,GameAudioService audioService)
    {
        _unit = unit;
        _origin = origin;
        _audioService = audioService;
    }

    public void PerformAttack(Health target)
    {
        if (!CanAct())
            return;

        if (target == null || target.IsDead)
            return;

        _unit.AnimatorBridge.PlayAttack();

        if (_unit.Config.ArrowPrefab != null)
        {
            ShootArrow(target);
            return;
        }

        PlayAttackSound(SoundId.MeleeHit);
        target.TakeDamage(_unit.Config.Damage);
    }

    public void PerformHeal(Health target)
    {
        if (!CanAct())
            return;

        if (target == null || target.IsDead)
            return;

        _unit.AnimatorBridge.PlayAttack();

        PlayAttackSound(SoundId.Heal);
        target.Heal(_unit.Config.HealAmount);
    }

    public float GetDistanceToTarget(Health target)
    {
        if (target == null)
            return float.MaxValue;

        Collider2D targetCollider = target.GetComponent<Collider2D>();


        if (_unit.BodyCollider != null && targetCollider != null)
        {
            ColliderDistance2D distance = _unit.BodyCollider.Distance(targetCollider);
            return Mathf.Max(0f, distance.distance);
        }

        return Vector2.Distance(_origin.position, target.transform.position);
    }

    private void ShootArrow(Health target)
    {
        ProjectileArrow arrow = Object.Instantiate(
            _unit.Config.ArrowPrefab,
            _origin.position,
            Quaternion.identity);

        arrow.Initialize(
            target,
            _unit.Config.Damage,
            _unit.Config.ArrowSpeed);

        PlayAttackSound(SoundId.ArrowShoot);
    }

    private void PlayAttackSound(SoundId soundId)
    {
        _audioService.PlayWorldSound(soundId, _origin.position);
    }

    private bool CanAct()
    {
        return _unit != null &&
               !_unit.IsDead &&
               _unit.Config != null &&
               _unit.AnimatorBridge != null;
    }
}