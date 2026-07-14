/// <summary>
/// Локализация названий профессий worker для UI.
/// </summary>
public static class WorkerJobLocalization
{
    public static string GetName(WorkerJobType jobType)
    {
        return jobType switch
        {
            WorkerJobType.None => Lang.Pick("Без работы", "No job"),
            WorkerJobType.ChopWood => Lang.Pick("Рубка дерева", "Chop wood"),
            WorkerJobType.MineGold => Lang.Pick("Добыча золота", "Mine gold"),
            WorkerJobType.HuntMeat => Lang.Pick("Охота", "Hunt"),
            _ => Lang.Pick("Неизвестно", "Unknown")
        };
    }
}
