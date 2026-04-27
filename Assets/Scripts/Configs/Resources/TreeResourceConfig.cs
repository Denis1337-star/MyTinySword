using UnityEngine;

/// <summary>
/// Определяет награду
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Tree Config")]
public class TreeResourceConfig : ResourceConfig
{
    [SerializeField] private int rewardAmount;
    public int RewardAmount => rewardAmount;

    public override bool IsValid()
    {
        return Priority >= 0f &&
               RespawnTime >= 0.1f &&
               rewardAmount >= 1;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        rewardAmount = Mathf.Max(1, rewardAmount);
    }
}