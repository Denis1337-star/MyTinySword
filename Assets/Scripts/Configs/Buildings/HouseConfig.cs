using UnityEngine;

/// <summary>
/// Конфиг дома
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/House Config")]
public sealed class HouseConfig : BaseConfig
{
    [SerializeField, Min(0)] private int _startWorkers;
    [SerializeField, Min(1)] private int _maxWorkers;
    [SerializeField, Min(0)] private int _baseWoodCost;
    [SerializeField, Min(0)] private int _baseGoldCost;
    [SerializeField, Min(0)] private int _woodIncreasePerWorker;
    [SerializeField, Min(0)] private int _goldIncreasePerWorker;

    public int StartWorkers => _startWorkers;
    public int MaxWorkers => _maxWorkers;

    public int BaseWoodCost => _baseWoodCost;
    public int BaseGoldCost => _baseGoldCost;

    public int WoodIncreasePerWorker => _woodIncreasePerWorker;
    public int GoldIncreasePerWorker => _goldIncreasePerWorker;

    public override bool IsValid()
    {
        return _startWorkers >= 0 &&
               _maxWorkers >= 1 &&
               _startWorkers <= _maxWorkers &&
               _baseWoodCost >= 0 &&
               _baseGoldCost >= 0 &&
               _woodIncreasePerWorker >= 0 &&
               _goldIncreasePerWorker >= 0;
    }

    private void OnValidate()
    {
        _startWorkers = Mathf.Max(0, _startWorkers);
        _maxWorkers = Mathf.Max(1, _maxWorkers);

        if (_startWorkers > _maxWorkers)
            _startWorkers = _maxWorkers;

        _baseWoodCost = Mathf.Max(0, _baseWoodCost);
        _baseGoldCost = Mathf.Max(0, _baseGoldCost);

        _woodIncreasePerWorker = Mathf.Max(0, _woodIncreasePerWorker);
        _goldIncreasePerWorker = Mathf.Max(0, _goldIncreasePerWorker);
    }
}