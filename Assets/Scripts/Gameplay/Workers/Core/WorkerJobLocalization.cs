/// <summary>
/// Локализация названий профессий worker'а для UI.
/// </summary>
public static class WorkerJobLocalization
{
    public static string GetName(WorkerJobType jobType)
    {
        return jobType switch
        {
            WorkerJobType.None => "Без работы",
            WorkerJobType.ChopWood => "Рубка дерева",
            WorkerJobType.MineGold => "Добыча золота",
            WorkerJobType.HuntMeat => "Охота",
            _ => "Неизвестно"
        };
    }

    public static string GetShortName(WorkerJobType jobType)
    {
        return jobType switch
        {
            WorkerJobType.None => "Idle",
            WorkerJobType.ChopWood => "Wood",
            WorkerJobType.MineGold => "Gold",
            WorkerJobType.HuntMeat => "Meat",
            _ => "Unknown"
        };
    }
}
