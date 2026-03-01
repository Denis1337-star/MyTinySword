using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// RTS Selection System (Android only):
/// - Tap по юниту → выделить
/// - Tap по земле → снять выделение
/// </summary>
public class SelectionSystem : MonoBehaviour
{
    //Список всех выделенных юнитов
    private readonly List<UnitSelectable> selectedUnits = new();
    private Camera cam;

    [SerializeField] private WorkerCommandPanel workerCommandPanel;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current == null)  //Проверка
            return;

        var touch = Touchscreen.current.primaryTouch;  //получает данные о первом косание 

        if (touch.press.wasPressedThisFrame)  //проверка на косание 
        {
            Vector2 screenPos = touch.position.ReadValue();  //счтитывает позицию
            ProcessTap(screenPos);
        }
    }

    private void ProcessTap(Vector2 screenPos)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider == null)
            return;

        UnitSelectable unit = hit.collider.GetComponent<UnitSelectable>();
        if (unit == null)
            return;

        if (selectedUnits.Contains(unit))
            return;

        SelectUnit(unit);

        var worker = unit.GetComponent<Worker>();
        if (worker != null)
        {
            workerCommandPanel.ShowForWorker(worker);
        }
        else
        {
            workerCommandPanel.Hide();
        }
    }

    private void SelectUnit(UnitSelectable unit)
    {
        selectedUnits.Add(unit);  //добовляет в спиоск выделеных юнитов
        unit.Select();  //вызуал отображения у самого юнита
    }

    public void ClearSelection()
    {
        foreach (var unit in selectedUnits)  //проходит по списку и снимает выделения
            unit.Deselect();

        selectedUnits.Clear();
    }

    /// <summary>
    /// Используем дальше для команд (Move, Attack и т.д.)
    /// </summary>
    public IReadOnlyList<UnitSelectable> GetSelectedUnits()
    {
        return selectedUnits;
    }
}
