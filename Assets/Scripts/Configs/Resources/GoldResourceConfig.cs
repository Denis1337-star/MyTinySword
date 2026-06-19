using UnityEngine;

/// <summary>
/// Конфиг золота
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Gold Config")]
public sealed class GoldResourceConfig : ResourceConfig
{
    [SerializeField, Min(0.1f)] private float _growInterval;

    public float GrowInterval => _growInterval;

    public override bool IsValid()
    {
        return base.IsValid() &&
               _growInterval >= 0.1f;
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        _growInterval = Mathf.Max(0.1f, _growInterval);
    }
}