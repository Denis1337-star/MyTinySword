using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Zenject;

/// <summary>
/// Центральная точка обработки gameplay pointer input.
/// Поддерживает touch на телефоне и mouse click на ПК/WebGL.
/// </summary>
public sealed class GameplayInputController : MonoBehaviour
{
    private SelectionSystem _selectionSystem;
    private CommandSystem _commandSystem;
    private EnemyHealthInspectService _enemyHealthInspectService;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        CommandSystem commandSystem,
        EnemyHealthInspectService enemyHealthInspectService)
    {
        _selectionSystem = selectionSystem;
        _commandSystem = commandSystem;
        _enemyHealthInspectService = enemyHealthInspectService;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (!GameplayPointerUtility.TryGetEndedTap(out GameplayPointer pointer))
            return;

        if (GameplayPointerUtility.IsPointerOverUI(pointer))
            return;

        HandleGameplayTap(pointer.ScreenPosition);
    }

    private void HandleGameplayTap(Vector2 screenPosition)
    {
        if (TutorialInputGuard.IsActive && !TutorialInputGuard.AllowsWorldGameplayInput())
            return;

        bool enemyInspected = false;

        if (TutorialInputGuard.AllowsEnemyInspect())
        {
            enemyInspected = _enemyHealthInspectService.TryInspectEnemyAtScreenPosition(
                screenPosition,
                out _);
        }
        else
        {
            _enemyHealthInspectService.HideCurrentInspect();
        }

        if (enemyInspected)
        {
            if (TutorialInputGuard.AllowsAttackCommand())
                _commandSystem.TryAttackSelectedArmyAtScreenPosition(screenPosition);

            return;
        }

        _enemyHealthInspectService.HideCurrentInspect();

        if (_selectionSystem.TrySelectAtScreenPosition(screenPosition))
            return;

        if (TutorialInputGuard.AllowsArmyMoveCommand() &&
            _commandSystem.TryMoveSelectedArmyAtScreenPosition(screenPosition))
            return;

        if (TutorialInputGuard.AllowsClearSelection())
            _selectionSystem.ClearSelection();
    }
}