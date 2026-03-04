

public static class WorkerJobLocalization 
{
    public static string GetName(WorkerJobType job)
    {
        return job switch
        {
            WorkerJobType.None => "Без работы",
            WorkerJobType.ChopWood => "Дровосек",
            WorkerJobType.MineGold => "Шахтёр",
            WorkerJobType.HuntMeat => "Охотник",
            _ => "Неизвестно"
        };
    }
}
