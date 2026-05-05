using UnityEngine;

/// <summary>
/// Конфиг рабочего
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Worker Config")]
public sealed class WorkerConfig : BaseConfig
{
    [SerializeField, Min(0.05f)] private float _reachResourceDistance;
    [SerializeField, Min(0.05f)] private float _maxWorkDistance;

    public float ReachResourceDistance => _reachResourceDistance;
    public float MaxWorkDistance => _maxWorkDistance;

    public override bool IsValid()
    {
        return _reachResourceDistance >= 0.05f &&
               _maxWorkDistance >= 0.05f &&
               _maxWorkDistance >= _reachResourceDistance;
    }

    private void OnValidate()
    {
        _reachResourceDistance = Mathf.Max(0.05f, _reachResourceDistance);
        _maxWorkDistance = Mathf.Max(0.05f, _maxWorkDistance);

        if (_maxWorkDistance < _reachResourceDistance)
            _maxWorkDistance = _reachResourceDistance;
    }
}