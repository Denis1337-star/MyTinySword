using UnityEngine;

/// <summary>
/// Конфиг овцы как ресурс
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Sheep Config")]
public sealed class SheepResourceConfig : ResourceConfig
{
    [SerializeField, Min(1)] private int _meatAmount;

    public int MeatAmount => _meatAmount;

    public override bool IsValid()
    {
        return base.IsValid() &&
               _meatAmount >= 1;
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        _meatAmount = Mathf.Max(1, _meatAmount);
    }
}