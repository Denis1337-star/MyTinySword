using UnityEngine;

/// <summary>
/// Простой инвентарь worker'а
/// Хранит количество ресурса, которое worker сейчас несёт
/// </summary>
public class WorkerInventory : MonoBehaviour
{
    public int CarriedAmount { get; private set; }  //Колличество переносимого ресурса

    public bool HasCargo => CarriedAmount > 0;  //Есть ли сейчас груз у worker

    /// <summary>
    /// Устанавливает количество текущего груза
    /// Отрицательные значения автоматически обрезаются до нуля
    /// </summary>
    public void SetCargo(int amount)
    {
        CarriedAmount = Mathf.Max(0, amount);
    }

    /// <summary>
    /// Забирает весь текущий груз и очищает инвентарь
    /// </summary>
    public int TakeCargo()
    {
        int amount = CarriedAmount;
        CarriedAmount = 0;
        return amount;
    }

    /// <summary>
    /// Полностью очищает инвентарь
    /// </summary>
    public void Clear()
    {
        CarriedAmount = 0;
    }
}
