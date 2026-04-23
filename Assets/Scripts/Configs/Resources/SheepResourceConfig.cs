using UnityEngine;

/// <summary>
/// Определяет время "добычи" и количество мяса
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Sheep Config")]
public class SheepResourceConfig : ResourceConfig
{
    [Min(0.1f)]
    [SerializeField] private float workTime;

    [Min(1)]
    [SerializeField] private int meatAmount;

    public float WorkTime => workTime;
    public int MeatAmount => meatAmount;

    public override bool IsValid()
    {
        return Priority >= 0f &&
               RespawnTime >= 0.1f &&
               workTime >= 0.1f &&
               meatAmount >= 1;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        workTime = Mathf.Max(0.1f, workTime);
        meatAmount = Mathf.Max(1, meatAmount);
    }
}
