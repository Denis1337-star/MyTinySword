using System;


/// <summary>
/// Главная база игрока или врага.
/// Её уничтожение завершает матч.
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
