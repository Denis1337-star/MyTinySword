using UnityEngine;

/// <summary>
/// Хранит параметры стартового количества worker
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/House Config")]
public class HouseConfig : BaseConfig
{
    [Header("Workers")]
    [Min(0)]
    [SerializeField] private int startWorkers = 1;

    [Min(1)]
    [SerializeField] private int maxWorkers = 5;

    [Header("Hire Cost")]
    [Min(0)]
    [SerializeField] private int baseWoodCost = 10;

    [Min(0)]
    [SerializeField] private int baseGoldCost = 10;

    [Min(0)]
    [SerializeField] private int woodIncreasePerWorker = 2;

    [Min(0)]
    [SerializeField] private int goldIncreasePerWorker = 2;

    public int StartWorkers => startWorkers;
    public int MaxWorkers => maxWorkers;
    public int BaseWoodCost => baseWoodCost;
    public int BaseGoldCost => baseGoldCost;
    public int WoodIncreasePerWorker => woodIncreasePerWorker;
    public int GoldIncreasePerWorker => goldIncreasePerWorker;

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
