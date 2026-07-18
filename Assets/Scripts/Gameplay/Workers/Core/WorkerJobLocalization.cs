/// <summary>
/// Локализация названий профессий worker для UI.
/// </summary>
public static class WorkerJobLocalization
{
    public static string GetName(WorkerJobType jobType)
    {
        return jobType switch
        {
            WorkerJobType.None => Lang.Pick("Без работы", "Idle"),
            WorkerJobType.ChopWood => Lang.Pick("Рубка дерева", "Chop Wood"),
            WorkerJobType.MineGold => Lang.Pick("Добыча золота", "Mine Gold"),
            WorkerJobType.HuntMeat => Lang.Pick("Охота", "Hunt"),
            _ => Lang.Pick("Неизвестно", "Unknown")
        };
    }
}
