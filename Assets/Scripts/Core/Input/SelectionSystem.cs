using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class SelectionSystem : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private WorkerCommandPanel workerCommandPanel;
    [SerializeField] private HousePanel housePanel;

    [Header("Raycast")]
    [SerializeField] private LayerMask ignoreRaycastLayer;

    private Camera cam;
    private UnitSelectable currentSelection;
    private CameraFocusController focusController;

    private readonly List<UnitSelectable> selectedUnits = new();

    private void Awake()
    {
        cam = Camera.main;
        focusController = GameServices.Instance.GetComponent<CameraFocusController>();
    }

    private void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
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
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        int mask = ~ignoreRaycastLayer.value;

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, mask);

        if (hit.collider != null)
        {
            UnitSelectable selectable = hit.collider.GetComponentInParent<UnitSelectable>();
            if (selectable != null)
            {
                Select(selectable);
                return;
            }

            HouseSelectable houseSelectable = hit.collider.GetComponentInParent<HouseSelectable>();
            if (houseSelectable != null)
            {
                ClearSelection();
                housePanel.Show(houseSelectable.GetHouse());
                return;
            }
        }
        ClearSelection();
    }

    private void Select(UnitSelectable selectable)
    {
        if (selectable == null)
            return;

        Worker worker = selectable.GetComponentInParent<Worker>();
        House house = selectable.GetComponentInParent<House>();

        // Если тыкнули в уже выбранный объект — просто снова показать правильную панель
        if (currentSelection == selectable)
        {
            if (worker != null)
            {
                workerCommandPanel.ShowForWorker(worker);
                focusController?.FocusOn(worker.transform);
                return;
            }

            if (house != null)
            {
                housePanel.Show(house);
                return;
            }

            return;
        }

        ClearSelection();

        currentSelection = selectable;
        selectedUnits.Add(selectable);
        selectable.Select();

        if (worker != null)
        {
            workerCommandPanel.ShowForWorker(worker);
            focusController?.FocusOn(worker.transform);
            return;
        }

        if (house != null)
        {
            housePanel.Show(house);
        }

    }

    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
            return;

        UnitSelectable selectable = worker.GetComponentInParent<UnitSelectable>();
        if (selectable != null)
            Select(selectable);
    }

    public void ClearSelection()
    {
        foreach (var unit in selectedUnits)
            unit.Deselect();

        selectedUnits.Clear();
        currentSelection = null;

        if (workerCommandPanel != null && workerCommandPanel.gameObject.activeSelf)
            workerCommandPanel.Hide();

        if (housePanel != null && housePanel.gameObject.activeSelf)
            housePanel.Hide();

        focusController?.CancelFocus();
    }

    public IReadOnlyList<UnitSelectable> GetSelectedUnits()
    {
        return selectedUnits;
    }
}

