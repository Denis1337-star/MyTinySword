using UnityEngine;

/// <summary>
/// Хранит параметры стартового количества worker'ов
/// максимального лимита и экономики найма
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/House Config")]
public class HouseConfig : BaseConfig
{
    [Header("Workers")]
    [Min(0)] public int startWorkers = 1;
    [Min(1)] public int maxWorkers = 5;

    [Header("Hire Cost")]
    [Min(0)] public int baseWoodCost = 10;
    [Min(0)] public int baseGoldCost = 10;
    [Min(0)] public int woodIncreasePerWorker = 2;
    [Min(0)] public int goldIncreasePerWorker = 2;
    /// <summary>
    /// Проверяет логическую корректность конфига.
    /// </summary>
    public override bool IsValid()
    {
        return startWorkers >= 0 &&
               maxWorkers >= 1 &&
               startWorkers <= maxWorkers &&
               baseWoodCost >= 0 &&
               baseGoldCost >= 0 &&
               woodIncreasePerWorker >= 0 &&
               goldIncreasePerWorker >= 0;
    }
    /// <summary>
    /// Автоматически исправляет некорректные значения в инспекторе.
    /// </summary>
    private void OnValidate()
    {
        startWorkers = Mathf.Max(0, startWorkers);
        maxWorkers = Mathf.Max(1, maxWorkers);

        if (startWorkers > maxWorkers)
            startWorkers = maxWorkers;

        baseWoodCost = Mathf.Max(0, baseWoodCost);
        baseGoldCost = Mathf.Max(0, baseGoldCost);
        woodIncreasePerWorker = Mathf.Max(0, woodIncreasePerWorker);
        goldIncreasePerWorker = Mathf.Max(0, goldIncreasePerWorker);
    }
}
