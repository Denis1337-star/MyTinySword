using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Кнопка запуска первого доступного уровня из LevelCatalog.
/// Используется в MainMenu вместо прямой загрузки сцены строкой.
/// </summary>
public sealed class StartFirstLevelButton : ValidatedMonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private LevelCatalog _levelCatalog;

    private LevelLoaderService _levelLoaderService;

    [Inject]
    private void Construct(LevelLoaderService levelLoaderService)
    {
        _levelLoaderService = levelLoaderService;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(LoadFirstLevel);
        RefreshInteractable();
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(LoadFirstLevel);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _button, nameof(_button));
        valid &= ValidationUtility.IsAssigned(this, _levelCatalog, nameof(_levelCatalog));

        if (_levelCatalog != null)
            valid &= _levelCatalog.IsValid();

        return valid;
    }

    private void LoadFirstLevel()
    {
        LevelConfig firstLevel = _levelCatalog.GetFirstLevel();

        if (firstLevel == null)
        {
            Debug.LogError($"{name}: первый уровень не найден в LevelCatalog.", this);
            return;
        }

        _levelLoaderService.TryLoadLevel(firstLevel);
    }

    private void RefreshInteractable()
    {
        if (_levelCatalog == null)
        {
            _button.interactable = false;
            return;
        }

        LevelConfig firstLevel = _levelCatalog.GetFirstLevel();
        _button.interactable = firstLevel != null && _levelLoaderService.CanLoadLevel(firstLevel);
    }
}