using System;


/// <summary>
/// Главная база 
/// </summary>
public class Castle : BuildingBase
{
    public event Action<Castle> OnCastleDestroyed;

    protected override void HandleDeath()
    {
        OnCastleDestroyed?.Invoke(this);
        base.HandleDeath();
    }
}
