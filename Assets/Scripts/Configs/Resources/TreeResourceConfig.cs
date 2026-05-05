using UnityEngine;

/// <summary>
/// Конфиг дерева
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Tree Config")]
public sealed class TreeResourceConfig : ResourceConfig
{
    [SerializeField, Min(1)] private int _rewardAmount;

    public int RewardAmount => _rewardAmount;

    public override bool IsValid()
    {
        return base.IsValid() &&
               _rewardAmount >= 1;
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        _rewardAmount = Mathf.Max(1, _rewardAmount);
    }
}