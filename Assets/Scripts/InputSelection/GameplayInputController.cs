using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Zenject;

/// <summary>
/// Центральная точка обработки gameplay pointer input
/// </summary>
public sealed class GameplayInputController : MonoBehaviour
{
    private SelectionSystem _selectionSystem;
    private CommandSystem _commandSystem;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        CommandSystem commandSystem)
    {
        _selectionSystem = selectionSystem;
        _commandSystem = commandSystem;
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
        // Если выбрана армия и игрок нажал по врагу — это команда атаки
        if (_commandSystem.TryAttackSelectedArmyAtScreenPosition(screenPosition))
            return;

        // Если под нажатием есть selectable объект — выбираем его
        if (_selectionSystem.TrySelectAtScreenPosition(screenPosition))
            return;

        // Если selectable объекта нет но выбрана армия — двигаем армию в точку
        if (_commandSystem.TryMoveSelectedArmyAtScreenPosition(screenPosition))
            return;

        // Если это не UI не враг не selectable и не команда армии — очищаем выбор
        _selectionSystem.ClearSelection();
    }
}