/// <summary>
/// Локализация названий профессий worker для UI
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
}
