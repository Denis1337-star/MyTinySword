using UnityEngine;

/// <summary>
/// Хранит параметры дистанции, которые управляют переходом
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Worker Config")]
public class WorkerConfig : BaseConfig
{
    [Header("Navigation")]
    [SerializeField] private float reachResourceDistance = 0.3f;  //Дистанция что дошел до ресурса
    [SerializeField] private float maxWorkDistance = 0.35f;  //На какой дистанции можно работать

    public float ReachResourceDistance => reachResourceDistance;
    public float MaxWorkDistance => maxWorkDistance;

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
