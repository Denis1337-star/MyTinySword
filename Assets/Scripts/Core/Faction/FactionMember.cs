using UnityEngine;

/// <summary>
/// Компонент принадлежности объекта к определённой фракции
/// Вешается на юнитов и здания, чтобы понимать
/// кто является союзником, врагом или нейтральным объектом
/// </summary>
public class FactionMember : MonoBehaviour
{
    [SerializeField] private FactionType faction = FactionType.Neutral;

    /// <summary>
    /// Текущая фракция объекта
    /// </summary>
    public FactionType Faction => faction;

    /// <summary>
    /// Проверяет, является ли другой объект врагом
    /// </summary>
    public bool IsEnemy(FactionMember other)
    {
        if (other == null)
            return false;

        if (Faction == FactionType.Neutral || other.Faction == FactionType.Neutral)
            return false;

        return Faction != other.Faction;
    }

    /// <summary>
    /// Проверяет, является ли другой объект союзником
    /// </summary>
    public bool IsAlly(FactionMember other)
    {
        if (other == null)
            return false;

        return Faction == other.Faction;
    }
}