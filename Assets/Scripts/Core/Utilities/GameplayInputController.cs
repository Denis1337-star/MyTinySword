using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Zenject;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Центральная точка обработки gameplay touch input
/// </summary>
public sealed class GameplayInputController : MonoBehaviour
{
    [SerializeField] private SelectionSystem _selectionSystem;
    [SerializeField] private CommandSystem _commandSystem;

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
        if (!TouchUtility.TryGetEndedTap(out Touch touch))
            return;

        if (TouchUtility.IsPointerOverUI(touch))
            return;

        HandleGameplayTap(touch.screenPosition);
    }

    private void HandleGameplayTap(Vector2 screenPosition)
    {
        // Если выбрана армия и игрок тапнул по врагу — это команда атаки
        if (_commandSystem != null &&
            _commandSystem.TryAttackSelectedArmyAtScreenPosition(screenPosition))
        {
            return;
        }

        // Если под tap есть selectable объект — выбираем его
        if (_selectionSystem != null &&
            _selectionSystem.TrySelectAtScreenPosition(screenPosition))
        {
            return;
        }

        // Если selectable объекта нет но выбрана армия — двигаем армию в точку
        if (_commandSystem != null &&
            _commandSystem.TryMoveSelectedArmyAtScreenPosition(screenPosition))
        {
            return;
        }

        // Если это не UI не враг не selectable и не команда армии — очищаем выбор
        _selectionSystem?.ClearSelection();
    }
}