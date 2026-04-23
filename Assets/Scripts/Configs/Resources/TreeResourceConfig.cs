using UnityEngine;

/// <summary>
/// Определяет время рубки и награду
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Tree Config")]
public class TreeResourceConfig : ResourceConfig
{
    [Min(0.1f)]
    [SerializeField] private float chopTime;

    [Min(1)]
    [SerializeField] private int rewardAmount;

    public float ChopTime => chopTime;
    public int RewardAmount => rewardAmount;

    public override bool IsValid()
    {
        return Priority >= 0f &&
               RespawnTime >= 0.1f &&
               chopTime >= 0.1f &&
               rewardAmount >= 1;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        chopTime = Mathf.Max(0.1f, chopTime);
        rewardAmount = Mathf.Max(1, rewardAmount);
    }
}