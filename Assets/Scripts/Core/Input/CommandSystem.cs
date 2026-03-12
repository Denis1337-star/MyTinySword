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
        if (Touch.activeTouches.Count == 0)  //если нет косаний = выход
            return;

        var touch = Touch.activeTouches[0];  //первое активное косание

        // команда “ќЋ№ ќ по тапу
        if (touch.phase != TouchPhase.Ended)
            return;

        // если UI Ч выходим
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touch.touchId))
            return;

        Vector2 worldPos = cam.ScreenToWorldPoint(touch.screenPosition);  //экранные в мировые сцены координаты

        // если тап по юниту Ч Ќ≈ двигаем
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);  //проверка если там юнит 
        if (hit.collider != null && hit.collider.GetComponent<UnitSelectable>() != null)  //выделеем 
            return;

        IssueMoveCommand(worldPos);  //команды идти в точку 
    }

    private void IssueMoveCommand(Vector2 targetPos)
    {
        var selectedUnits = selectionSystem.GetSelectedUnits();
        if (selectedUnits.Count == 0)
            return;

        foreach (var selectable in selectedUnits)
        {
            // –абочих руками не двигаем
            if (selectable.TryGetComponent(out Worker worker))
                continue;

            var movement = selectable.GetComponent<UnitMovement>();
            if (movement != null)
                movement.MoveTo(targetPos);
        }
    }
}
