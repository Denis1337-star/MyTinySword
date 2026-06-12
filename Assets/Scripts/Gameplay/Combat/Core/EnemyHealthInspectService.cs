using UnityEngine;

/// <summary>
/// Отвечает за просмотр HP врагов по клику.
/// Не выбирает врага как управляемый объект, а только включает его HP bar.
/// </summary>
public sealed class EnemyHealthInspectService
{
    private const int HitBufferSize = 16;

    private readonly Collider2D[] _hitBuffer = new Collider2D[HitBufferSize];

    private readonly Camera _mainCamera;

    private HealthBarSpawner _currentInspectedBar;

    public EnemyHealthInspectService(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }

    public bool TryInspectEnemyAtScreenPosition(Vector2 screenPosition, out Health inspectedHealth)
    {
        inspectedHealth = null;

        if (_mainCamera == null)
            return false;

        Vector2 worldPosition = TouchUtility.ScreenToWorld(_mainCamera, screenPosition);

        int hitCount = Physics2D.OverlapPointNonAlloc(
            worldPosition,
            _hitBuffer);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = _hitBuffer[i];

            if (hit == null)
                continue;

            if (!TryGetEnemyHealthFromHit(hit, out Health health, out HealthBarSpawner healthBarSpawner))
                continue;

            Inspect(healthBarSpawner);
            inspectedHealth = health;
            return true;
        }

        HideCurrentInspect();
        return false;
    }

    public void HideCurrentInspect()
    {
        if (_currentInspectedBar == null)
            return;

        _currentInspectedBar.HideInspected();
        _currentInspectedBar = null;
    }

    private void Inspect(HealthBarSpawner healthBarSpawner)
    {
        if (healthBarSpawner == null)
            return;

        if (_currentInspectedBar != null && _currentInspectedBar != healthBarSpawner)
            _currentInspectedBar.HideInspected();

        _currentInspectedBar = healthBarSpawner;
        _currentInspectedBar.ShowInspected();
    }

    private bool TryGetEnemyHealthFromHit(
        Collider2D hit,
        out Health health,
        out HealthBarSpawner healthBarSpawner)
    {
        health = null;
        healthBarSpawner = null;

        Health foundHealth = hit.GetComponentInParent<Health>();

        if (foundHealth == null || foundHealth.IsDead)
            return false;

        FactionMember factionMember = foundHealth.GetComponentInParent<FactionMember>();

        if (factionMember == null || !factionMember.IsEnemy())
            return false;

        HealthBarSpawner foundSpawner = foundHealth.GetComponentInParent<HealthBarSpawner>();

        if (foundSpawner == null)
            return false;

        health = foundHealth;
        healthBarSpawner = foundSpawner;
        return true;
    }
}