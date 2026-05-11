using System;

/// <summary>
/// Главное здание базы
/// </summary>
public sealed class Castle : BuildingBase
{
    public event Action<Castle> OnCastleDestroyed;

    protected override void HandleDeath()
    {
        OnCastleDestroyed?.Invoke(this);

        base.HandleDeath();
    }
}