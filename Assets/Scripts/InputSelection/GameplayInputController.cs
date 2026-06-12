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
        bool enemyInspected = _enemyHealthInspectService.TryInspectEnemyAtScreenPosition(
            screenPosition,
            out _);

        if (enemyInspected)
        {
            // Если выбрана армия и игрок нажал по врагу —
            // отдаём команду атаки и оставляем HP врага показанным.
            _commandSystem.TryAttackSelectedArmyAtScreenPosition(screenPosition);
            return;
        }

        // Если кликнули не по врагу — скрываем HP, которое было показано inspect-кликом.
        _enemyHealthInspectService.HideCurrentInspect();

        // Если под нажатием есть selectable объект игрока — выбираем его.
        if (_selectionSystem.TrySelectAtScreenPosition(screenPosition))
            return;

        // Если selectable объекта нет, но выбрана армия — двигаем армию в точку.
        if (_commandSystem.TryMoveSelectedArmyAtScreenPosition(screenPosition))
            return;

        // Если это не UI, не враг, не selectable и не команда армии — очищаем выбор.
        _selectionSystem.ClearSelection();
    }
}