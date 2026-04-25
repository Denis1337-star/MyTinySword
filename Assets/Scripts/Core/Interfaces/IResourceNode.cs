using System;
using UnityEngine;

/// <summary>
/// Интерфейс ресурса
/// </summary>
public interface IResourceNode
{
    bool IsAvailable { get; }                                  //Доступен ли ресурс для работы
    float Priority { get; }                                    //Приоритет ресурса 
    bool HasFreeSlot();                                        //Есть ли свободные слоты для работы

    bool TryStartWork(Worker worker, Action<int> onFinished);  //Попытка начать работу
    Vector2 GetWorkPosition(Worker worker);                    //Получить позицию работы для конкретного worker
    void CancelWork(Worker worker);                            //Отменить работу worker

}



