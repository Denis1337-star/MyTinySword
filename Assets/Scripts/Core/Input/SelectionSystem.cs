using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using System.Collections.Generic;


public class SelectionSystem : MonoBehaviour
{
    private readonly List<UnitSelectable> selectedUnits = new();
    private Camera cam;

    [SerializeField] private WorkerCommandPanel workerCommandPanel;
    [SerializeField] private HousePanel housePanel;

    private void Awake()
    {
        cam = Camera.main;
        EnhancedTouchSupport.Enable();
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Touch.activeTouches.Count == 0)
            return;

        var touch = Touch.activeTouches[0];

        if (touch.phase != TouchPhase.Ended)
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touch.touchId))
            return;

        ProcessTap(touch.screenPosition);
    }

    private void ProcessTap(Vector2 screenPos)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider == null)
        {
            ClearSelection();
            HidePanels();
            return;
        }

        var houseSelectable = hit.collider.GetComponent<HouseSelectable>();
        if (houseSelectable != null)
        {
            ClearSelection();
            HidePanels();
            housePanel.Show(houseSelectable.GetHouse());
            return;
        }

        var unit = hit.collider.GetComponent<UnitSelectable>();
        if (unit != null)
        {
            ClearSelection();
            SelectUnit(unit);

            var worker = unit.GetComponent<Worker>();
            if (worker != null)
                workerCommandPanel.ShowForWorker(worker);

            return;
        }

        ClearSelection();
        HidePanels();
    }

    private void SelectUnit(UnitSelectable unit)
    {
        selectedUnits.Add(unit);
        unit.Select();
    }

    private void HidePanels()
    {
        workerCommandPanel?.Hide();
        housePanel?.Hide();
    }

    public void ClearSelection()
    {
        foreach (var unit in selectedUnits)
            unit.Deselect();

        selectedUnits.Clear();
    }

    public IReadOnlyList<UnitSelectable> GetSelectedUnits()
    {
        return selectedUnits;
    }
}
