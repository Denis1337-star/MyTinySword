using UnityEngine;

/// <summary>
/// Базовый конфиг для всех ресурсов
/// Содержит общие параметры, которые есть у любого типа ресурса
/// </summary>
public abstract class ResourceConfig : BaseConfig
{
    [Header("Common")]
    [Min(0f)] public float priority = 1f;
    [Min(0.1f)] public float respawnTime = 10f;

    /// <summary>
    /// Ограничивает общие значения ресурса в редакторе
    /// Наследники могут расширять эту проверку, вызывая base.OnValidate()
    /// </summary>
    protected virtual void OnValidate()
    {
        priority = Mathf.Max(0f, priority);
        respawnTime = Mathf.Max(0.1f, respawnTime);
    }
}

/// <summary>
/// Конфиг для дерева
/// Определяет время рубки и награду
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Tree Config")]
public class TreeResourceConfig : ResourceConfig
{
    [Min(0.1f)] public float chopTime = 2f;
    [Min(1)] public int rewardAmount = 3;

    public override bool IsValid()
    {
        return priority >= 0f &&
               respawnTime >= 0.1f &&
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


/// <summary>
/// Конфиг золотого ресурса
/// Определяет время добычи и интервал роста размера ресурса
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Gold Config")]
public class GoldResourceConfig : ResourceConfig
{
    [Min(0.1f)] public float mineTime = 3f;
    [Min(0.1f)] public float growInterval = 5f;

    public override bool IsValid()
    {
        return priority >= 0f &&
               respawnTime >= 0.1f &&
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

/// <summary>
/// Конфиг овцы как ресурса
/// Определяет время "добычи" и количество мяса
/// </summary>
[CreateAssetMenu(menuName = "MyTinySword/Configs/Resources/Sheep Config")]
public class SheepResourceConfig : ResourceConfig
{
    [Min(0.1f)] public float workTime = 2f;
    [Min(1)] public int meatAmount = 2;

    public override bool IsValid()
    {
        return priority >= 0f &&
               respawnTime >= 0.1f &&
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
