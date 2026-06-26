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
        valid &= ValidationUtility.IsValidConfig(this, _levelCatalog, nameof(_levelCatalog));

        return valid;
    }

    private void LoadFirstLevel()
    {
        _levelLoaderService.TryLoadLevel(_levelCatalog.GetFirstLevel());
    }

    private void RefreshInteractable()
    {
        _button.interactable = true;
    }
}