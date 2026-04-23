using UnityEngine;

/// <summary>
/// Отвечает за поиск лучшего ресурса для worker на основе приоритета и расстояния
/// </summary>
public static class ResourceFinder
{
    /// <summary>
    /// Вес приоритета в общей формуле оценки ресурса
    /// Чем выше значение, тем сильнее priority влияет на выбор
    /// </summary>
    private const float PriorityWeight = 100f;

    // Находит лучший доступный ресурс указанного типа
    public static T FindBest<T>(Vector2 from) where T : ResourceNodeBase
    {
        if (ResourceRegistry.Instance == null)
            return null;

        T best = null;
        float bestScore = float.MinValue;

        foreach (var node in ResourceRegistry.Instance.Nodes)
        {
            if (node is not T typed)    // Оставляем только ресурсы нужного типа
                continue;
             
            if (!typed.IsAvailable)        // Ресурс должен быть доступен для работы
                continue;

            if (!typed.HasFreeSlot())         // У ресурса должен быть хотя бы один свободный слот
                continue;

            float dist = Vector2.Distance(from, typed.WorkPosition);
            float score = typed.Priority * PriorityWeight - dist;

            if (score > bestScore)
            {
                bestScore = score;
                best = typed;
            }
        }

        return best;
    }
}
