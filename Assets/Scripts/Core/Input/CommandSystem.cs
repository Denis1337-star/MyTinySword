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
        var selectedUnits = selectionSystem.GetSelectedUnits();  //список выделеных юнитов 
        if (selectedUnits.Count == 0)  //если пустой выход
            return;

        float spacing = 0.8f;  //расто€ние между юнитами 

        var positions = FormationCalculator.GetSquareFormation(   //расчитываем позиции дл€ каждого юнита 
            targetPos,  //центр
            selectedUnits.Count,  //количество юнитов
            spacing  //рассто€ние между ними
        );

        for (int i = 0; i < selectedUnits.Count; i++)  //дл€ каждого юнита 
        {
            var movement = selectedUnits[i].GetComponent<UnitMovement>();  //получаем движение 
            if (movement != null)
                movement.MoveTo(positions[i]);
        }
    }
}
