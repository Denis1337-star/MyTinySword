using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// <summary>
/// Обрабатывает команды игрока после выбора объектов
/// Основная задача:
/// - отследить тап по игровому миру
/// - убедиться, что это не UI и не selectable-объект
/// - передать команду движения выбранным юнитам
/// </summary>
public class CommandSystem : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;

    private Camera mainCamera;


    private void Awake()
    {
        mainCamera = Camera.main;
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


        if (touch.phase != TouchPhase.Ended)
            return;


        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touch.touchId))
            return;

        Vector2 worldPos = mainCamera.ScreenToWorldPoint(touch.screenPosition); 


        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null)
        {
            UnitSelectable selectable = hit.collider.GetComponentInParent<UnitSelectable>();
            if (selectable != null)
                return;
        }

        IssueMoveCommand(worldPos); 
    }

    /// <summary>
    /// Отправляет команду движения всем выбранным объектам,
    /// у которых есть компонент UnitMovement.
    /// </summary>
    private void IssueMoveCommand(Vector2 targetPos)
    {
        var selectedUnits = selectionSystem.GetSelectedUnits();
        if (selectedUnits.Count == 0)
            return;

        foreach (var selectable in selectedUnits)
        {
            // Рабочих руками не двигаем
            if (selectable.TryGetComponent(out Worker worker))
                continue;

            var movement = selectable.GetComponent<UnitMovement>();
            if (movement != null)
                movement.MoveTo(targetPos);
        }
    }
}
