using UnityEngine;

/// <summary>
/// Конфиг одного уровня
/// </summary>
[CreateAssetMenu(
    fileName = "LevelConfig",
    menuName = "MyTinySword/Levels/Level Config")]
public sealed class LevelConfig : BaseConfig
{
    [Header("Identity")]
    [SerializeField] private string _levelId = "level_1";
    [SerializeField, Min(1)] private int _levelIndex = 1;

    [Header("Scene")]
    [SerializeField] private string _sceneName = "Level_1";

    [Header("View")]
    [SerializeField] private string _displayName = "Уровень 1";
    [SerializeField, TextArea] private string _description = "Первый уровень.";

    [Header("Tutorial")]
    [SerializeField] private bool _isTutorialLevel = true;

    public string LevelId => _levelId;
    public int LevelIndex => _levelIndex;
    public string SceneName => _sceneName;
    public string DisplayName => _displayName;
    public string Description => _description;
    public bool IsTutorialLevel => _isTutorialLevel;

    public override bool IsValid()
    {
        bool valid = true;

        if (string.IsNullOrWhiteSpace(_levelId))
        {
            Debug.LogError($"{name}: Level Id не задан.", this);
            valid = false;
        }

        if (_levelIndex < 1)
        {
            Debug.LogError($"{name}: Level Index должен быть больше 0.", this);
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(_sceneName))
        {
            Debug.LogError($"{name}: Scene Name не задан.", this);
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(_displayName))
        {
            Debug.LogError($"{name}: Display Name не задан.", this);
            valid = false;
        }

        return valid;
    }
}