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
        bool hasSelectable = _selectionSystem.TryGetSelectableAtScreenPosition(screenPosition,
            out UnitSelectable selectable);

        if (hasSelectable && selectable != null && selectable.CanBeSelected)
        {
            _selectionSystem.Select(selectable);
            return;
        }

        bool commandWasIssued = _commandSystem.TryCommandSelectedArmyAtScreenPosition(screenPosition);

        if (commandWasIssued)
            return;

        _selectionSystem.ClearSelection();
    }
}