using UnityEngine;

/// <summary>
/// кто является союзником, врагом или нейтральным объектом
/// </summary>
public class FactionMember : MonoBehaviour
{
    [SerializeField] private FactionType faction = FactionType.Player;

    public FactionType Faction => faction;

    public bool IsEnemy(FactionMember other)
    {
        if (other == null)
            return false;

        return faction != other.faction;
    }

    public bool IsAlly(FactionMember other)
    {
        if (other == null)
            return false;

        return faction == other.faction;
    }

    public bool IsPlayer()
    {
        return faction == FactionType.Player;
    }

    public bool IsEnemy()
    {
        return faction == FactionType.Enemy;
    }
}