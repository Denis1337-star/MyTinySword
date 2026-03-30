using System;
using UnityEngine;

/// <summary>
/// Интерфейс ресурса, с которым может взаимодействовать рабочий
/// </summary>
public interface IResourceNode
{
    bool IsAvailable { get; }                                  //Доступен ли ресурс для работы
    Vector2 WorkPosition { get; }                              //Базовая позиция для работы
    float Priority { get; }                                    //Приоритет ресурса (для выбора лучшего)
    bool HasFreeSlot();                                        //Есть ли свободные слоты для работы

    bool TryStartWork(Worker worker, Action<int> onFinished);  //Попытка начать работу
    Vector2 GetWorkPosition(Worker worker);                    //Получить позицию работы для конкретного worker
    void CancelWork(Worker worker);                            //Отменить работу worker

}

// Размер ресурса, влияет на количество добычи и визуал
public enum ResourceSize
{
    Tiny = 1,
    Small = 2,
    Medium = 3,
    Large = 4,
    Huge = 5,
    Giant = 6
}
// Тип ресурса, который может добывать и переносить рабочий
public enum ResourceType
{
    None,
    Wood,
    Gold,
    Meat
}
