using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Каталог всех уровней игры
/// </summary>
[CreateAssetMenu(
    fileName = "LevelCatalog",
    menuName = "MyTinySword/Levels/Level Catalog")]
public sealed class LevelCatalog : BaseConfig
{
    [SerializeField] private List<LevelConfig> _levels = new();

    public IReadOnlyList<LevelConfig> Levels => _levels;

    public LevelConfig GetFirstLevel()
    {
        return _levels[0];
    }

    public LevelConfig GetByIndex(int levelIndex)
    {
        for (int i = 0; i < _levels.Count; i++)
        {
            LevelConfig level = _levels[i];

            if (level.LevelIndex == levelIndex)
                return level;
        }

        return null;
    }

    public LevelConfig GetById(string levelId)
    {
        if (string.IsNullOrWhiteSpace(levelId))
            return null;

        for (int i = 0; i < _levels.Count; i++)
        {
            LevelConfig level = _levels[i];

            if (level.LevelId == levelId)
                return level;
        }

        return null;
    }

    public override bool IsValid()
    {
        bool valid = ValidationUtility.ValidList(this, _levels, nameof(_levels));

        if (!valid)
            return false;

        HashSet<string> ids = new();
        HashSet<int> indexes = new();

        for (int i = 0; i < _levels.Count; i++)
        {
            LevelConfig level = _levels[i];

            valid &= level.IsValid();

            if (!ids.Add(level.LevelId))
            {
                Debug.LogError($"{name}: повторяется Level Id: {level.LevelId}.", this);
                valid = false;
            }

            if (!indexes.Add(level.LevelIndex))
            {
                Debug.LogError($"{name}: повторяется Level Index: {level.LevelIndex}.", this);
                valid = false;
            }
        }

        return valid;
    }
}