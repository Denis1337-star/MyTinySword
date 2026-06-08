/// <summary>
/// Хранит информацию о текущем выбранном уровне во время runtime.
/// Заполняется перед загрузкой сцены через LevelLoaderService.
/// </summary>
public sealed class LevelRuntimeService
{
    public LevelConfig CurrentLevel { get; private set; }

    public bool HasCurrentLevel => CurrentLevel != null;

    public void SetCurrentLevel(LevelConfig levelConfig)
    {
        CurrentLevel = levelConfig;
    }

    public void Clear()
    {
        CurrentLevel = null;
    }
}