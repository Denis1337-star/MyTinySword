using UnityEngine;

[CreateAssetMenu(menuName = "MyTinySword/Configs/Worker Config")]
public class WorkerConfig : BaseConfig
{
    [Header("Navigation")]
    [Min(0.05f)] public float reachResourceDistance = 0.3f;
    [Min(0.05f)] public float maxWorkDistance = 0.35f;

    public override bool IsValid()
    {
        return reachResourceDistance >= 0.05f &&
               maxWorkDistance >= 0.05f &&
               maxWorkDistance >= reachResourceDistance;
    }

    private void OnValidate()
    {
        reachResourceDistance = Mathf.Max(0.05f, reachResourceDistance);
        maxWorkDistance = Mathf.Max(0.05f, maxWorkDistance);

        if (maxWorkDistance < reachResourceDistance)
            maxWorkDistance = reachResourceDistance;
    }
}
