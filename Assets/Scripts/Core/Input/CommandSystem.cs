using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class CommandSystem : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        EnhancedTouchSupport.Enable();
    }

    private void Update()
    {
        HandleMoveCommand();
    }

    private void HandleMoveCommand()
    {
        if (Touch.activeTouches.Count == 0)
            return;

        var touch = Touch.activeTouches[0];

        // команда “ќЋ№ ќ по тапу
        if (touch.phase != TouchPhase.Ended)
            return;

        // если UI Ч выходим
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touch.touchId))
            return;

        Vector2 worldPos = cam.ScreenToWorldPoint(touch.screenPosition);

        // если тап по юниту Ч Ќ≈ двигаем
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null && hit.collider.GetComponent<UnitSelectable>() != null)
            return;

        IssueMoveCommand(worldPos);
    }

    private void IssueMoveCommand(Vector2 targetPos)
    {
        var selectedUnits = selectionSystem.GetSelectedUnits();
        if (selectedUnits.Count == 0)
            return;

        float spacing = 0.8f;

        var positions = FormationCalculator.GetSquareFormation(
            targetPos,
            selectedUnits.Count,
            spacing
        );

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            var movement = selectedUnits[i].GetComponent<UnitMovement>();
            if (movement != null)
                movement.MoveTo(positions[i]);
        }
    }
}
