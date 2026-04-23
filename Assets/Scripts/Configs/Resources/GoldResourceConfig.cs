using UnityEngine;

/// <summary>
/// Определяет время добычи и интервал роста размера ресурса
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Gold Config")]
public class GoldResourceConfig : ResourceConfig
{
    [Min(0.1f)]
    [SerializeField] private float mineTime;

    [Min(0.1f)]
    [SerializeField] private float growInterval ;

    public float MineTime => mineTime;
    public float GrowInterval => growInterval;

    public override bool IsValid()
    {
        return Priority >= 0f &&
               RespawnTime >= 0.1f &&
               mineTime >= 0.1f &&
               growInterval >= 0.1f;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        mineTime = Mathf.Max(0.1f, mineTime);
        growInterval = Mathf.Max(0.1f, growInterval);
    }
}
