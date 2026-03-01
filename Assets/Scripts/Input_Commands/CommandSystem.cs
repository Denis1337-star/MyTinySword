using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// <summary>
/// Принимает input и отдаёт команды юнитам
/// </summary>
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
        // Если клик по UI — выходим
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Touch.activeTouches.Count == 0)
            return;

        var touch = Touch.activeTouches[0];

        if (touch.phase != TouchPhase.Began)
            return;

        Vector2 screenPos = touch.screenPosition;
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        // ПРОВЕРКА: попали ли мы в юнита?
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.GetComponent<UnitSelectable>() != null)
        {
            //  Клик по юниту — НИЧЕГО НЕ ДЕЛАЕМ
            return;
        }

        //  Клик по земле — команда движения
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
            var unit = selectedUnits[i];
            var movement = unit.GetComponent<UnitMovement>();

            if (movement != null)
            {
                movement.MoveTo(positions[i]);
            }
        }
    }
}
