using UnityEngine;

/// <summary>
/// Определяет  количество мяса
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Sheep Config")]
public class SheepResourceConfig : ResourceConfig
{
    [SerializeField] private int meatAmount;

    public int MeatAmount => meatAmount;

    public override bool IsValid()
    {
        return Priority >= 0f &&
               RespawnTime >= 0.1f &&
               meatAmount >= 1;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        meatAmount = Mathf.Max(1, meatAmount);
    }
}
