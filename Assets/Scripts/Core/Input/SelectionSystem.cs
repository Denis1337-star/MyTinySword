using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class SelectionSystem : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private WorkerCommandPanel workerCommandPanel;
    [SerializeField] private HousePanel housePanel;

    [Header("Raycast Ignore")]
    [SerializeField] private LayerMask ignoreLayer;

    //private readonly List<UnitSelectable> selectedUnits = new();
    private Camera cam;
    private UnitSelectable currentSelection;

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

        if (!touch.press.wasReleasedThisFrame)
            return;

        // Игнор кликов по UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        ProcessTap(touch.position.ReadValue());
    }

    private void ProcessTap(Vector2 screenPos)
    {

        Vector2 worldPoint = cam.ScreenToWorldPoint(screenPos);
        // ~ignoreLayer => все слои кроме игнорируемого
        int mask = ~ignoreLayer.value;

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, mask);

        if (hit.collider != null)
        {
            var selectable = hit.collider.GetComponent<UnitSelectable>();
            if (selectable != null)
            {
                SelectUnit(selectable);
                return;
            }
        }

        ClearSelection();

    }

    private void SelectUnit(UnitSelectable selectable)
    {
        if (currentSelection == selectable)
            return;

        ClearSelection();

        currentSelection = selectable;
        currentSelection.Select();

        // Worker
        if (selectable.TryGetComponent(out Worker worker))
        {
            workerCommandPanel.ShowForWorker(worker);

            var focus = FindAnyObjectByType<CameraFocusController>();
            if (focus != null)
                focus.FocusOn(worker.transform);
        }

        // House
        if (selectable.TryGetComponent(out House house))
        {
            housePanel.Show(house);
        }
    }

    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null) return;

        var selectable = worker.GetComponent<UnitSelectable>();
        if (selectable != null)
            SelectUnit(selectable);
    }

    public void ClearSelection()
    {
        if (currentSelection != null)
        {
            currentSelection.Deselect();
            currentSelection = null;
        }

        workerCommandPanel.Hide();
        housePanel.Hide();

        var focus = FindAnyObjectByType<CameraFocusController>();
        if (focus != null)
            focus.CancelFocus();
    }

    public IReadOnlyList<UnitSelectable> GetSelectedUnits()
    {
        if (currentSelection != null)
            return new List<UnitSelectable> { currentSelection };
        return new List<UnitSelectable>();
    }
}
