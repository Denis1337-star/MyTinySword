/// <summary>
/// Сравнение конфигов зданий по ссылке или BuildingId.
/// </summary>
public static class BuildingConfigUtility
{
    public static bool Matches(BuildingConfig required, BuildingConfig candidate)
    {
        if (required == null)
            return candidate != null;

        if (candidate == null)
            return false;

        return required == candidate ||
               required.BuildingId == candidate.BuildingId;
    }
}
