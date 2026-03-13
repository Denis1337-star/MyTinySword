using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
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

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    private void Awake()
    {
        cam = Camera.main;
        focusController = FindObjectOfType<CameraFocusController>();

        Debug.Log($"[SelectionSystem] Awake. Camera = {(cam != null ? cam.name : "NULL")}", this);
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
        {
            Debug.Log("[SelectionSystem] Touch over UI, ignore.");
            return;
        }

        ProcessTap(touch.screenPosition);
    }

    private void ProcessTap(Vector2 screenPos)
    {
        if (cam == null)
        {
            Debug.LogError("[SelectionSystem] Camera.main is NULL", this);
            return;
        }

        Vector3 worldPos3 = cam.ScreenToWorldPoint(screenPos);
        Vector2 worldPos = new Vector2(worldPos3.x, worldPos3.y);
        int mask = ~ignoreRaycastLayer.value;

        Debug.Log($"[SelectionSystem] Tap screen={screenPos} world={worldPos} mask={mask}");

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, mask);

        if (hit.collider == null)
        {
            Debug.Log("[SelectionSystem] Raycast hit NOTHING");
            ClearSelection();
            return;
        }

        Debug.Log($"[SelectionSystem] Hit collider = {hit.collider.name}, layer = {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

        UnitSelectable selectable = hit.collider.GetComponentInParent<UnitSelectable>();

        if (selectable == null)
        {
            Debug.Log("[SelectionSystem] UnitSelectable NOT found in parent chain");
            ClearSelection();
            return;
        }

        Debug.Log($"[SelectionSystem] Selectable found on {selectable.name}");
        Select(selectable);
    }

    private void Select(UnitSelectable selectable)
    {
        if (selectable == null)
            return;

        Worker worker = selectable.GetComponentInParent<Worker>();
        House house = selectable.GetComponentInParent<House>();

        Debug.Log($"[SelectionSystem] Select -> selectable={selectable.name}, worker={(worker != null ? worker.name : "null")}, house={(house != null ? house.name : "null")}");

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
        {
            if (unit != null)
                unit.Deselect();
        }

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

