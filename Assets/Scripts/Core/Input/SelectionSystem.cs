using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class SelectionSystem : MonoBehaviour
{
    private readonly List<UnitSelectable> selectedUnits = new();
    private Camera cam;

    [SerializeField] private WorkerCommandPanel workerCommandPanel;
    [SerializeField] private HousePanel housePanel;
    [SerializeField] private LayerMask ignoreLayers;

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

        if (Touchscreen.current == null)  //Проверка
            return;

        var touch = Touchscreen.current.primaryTouch;  //получает данные о первом касание 

        if (touch.press.wasPressedThisFrame)  //проверка на косание 
        {
            Vector2 screenPos = touch.position.ReadValue();  //счтитывает позицию
            ProcessTap(screenPos);
        }
    }

    private void ProcessTap(Vector2 screenPos)
    {

        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, ~ignoreLayers);

        if (hit.collider == null) return;

        var houseSelectable = hit.collider.GetComponent<HouseSelectable>();
        if (houseSelectable != null)
        {
            ClearSelection();
            HidePanels();
            housePanel.Show(houseSelectable.GetHouse());
            return;
        }

        UnitSelectable unit = hit.collider.GetComponent<UnitSelectable>();
        if (unit != null)
        {
            ClearSelection();
            SelectUnit(unit);

            var worker = unit.GetComponent<Worker>();
            if (worker != null)
                workerCommandPanel.ShowForWorker(worker);

            return;
        }
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
    public void SelectWorkerFromUI(Worker worker)
    {
        ClearSelection();

        var selectable = worker.GetComponent<UnitSelectable>();
        if (selectable == null)
            return;

        SelectUnit(selectable);

        if (workerCommandPanel != null)
            workerCommandPanel.ShowForWorker(worker);

        var cameraController = Camera.main.GetComponent<CameraInputController>();
        if (cameraController != null)
            cameraController.FocusOn(worker.transform);
    }
}
