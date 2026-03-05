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

    private Camera cam;
    private UnitSelectable currentSelection;
    private CameraFocusController focusController;

    private readonly List<UnitSelectable> selectedUnits = new();

    private void Awake()
    {
        cam = Camera.main;
        focusController = FindAnyObjectByType<CameraFocusController>();
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
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        // ~ignoreLayer => все слои кроме игнорируемого
        int mask = ~ignoreLayer.value;

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, mask);

        if (hit.collider != null &&
                  hit.collider.TryGetComponent(out UnitSelectable selectable))
        {
            Select(selectable);
            return;
        }

        ClearSelection();

    }

    private void Select(UnitSelectable selectable)
    {
        if (currentSelection == selectable)
            return;

        ClearSelection();

        currentSelection = selectable;
        selectable.Select();

        selectedUnits.Add(selectable);

        if (selectable.TryGetComponent(out Worker worker))
        {
            workerCommandPanel.ShowForWorker(worker);
            focusController?.FocusOn(worker.transform);
        }

        if (selectable.TryGetComponent(out House house))
        {
            housePanel.Show(house);
        }
    }

    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
            return;

        if (worker.TryGetComponent(out UnitSelectable selectable))
            Select(selectable);
    }

    public void ClearSelection()
    {
        if (currentSelection != null)
            currentSelection.Deselect();

        currentSelection = null;

        workerCommandPanel.Hide();
        housePanel.Hide();

        focusController?.CancelFocus();
    }
    public IReadOnlyList<UnitSelectable> GetSelectedUnits()
    {
        return selectedUnits;
    }
}

