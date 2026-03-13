using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class SelectionSystem : MonoBehaviour
{
    public event Action<UnitSelectable> SelectionChanged;
    public event Action SelectionCleared;

    [Header("Raycast")]
    [SerializeField] private LayerMask ignoreRaycastLayer;

    private Camera cam;
    private UnitSelectable currentSelection;
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

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.touchId))
            return;

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
        Vector2 worldPos = new(worldPos3.x, worldPos3.y);

        int mask = ~ignoreRaycastLayer.value;
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, mask);

        if (hit.collider == null)
        {
            ClearSelection();
            return;
        }

        UnitSelectable selectable = hit.collider.GetComponentInParent<UnitSelectable>();

        if (selectable == null)
        {
            ClearSelection();
            return;
        }

        Select(selectable);
    }

    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
            return;

        UnitSelectable selectable = worker.GetComponentInParent<UnitSelectable>();

        if (selectable == null)
        {
            Debug.LogWarning($"[SelectionSystem] Worker {worker.name} has no UnitSelectable in parents.", worker);
            return;
        }

        Select(selectable);
    }

    public void Select(UnitSelectable selectable)
    {
        if (selectable == null)
            return;

        if (currentSelection == selectable)
        {
            SelectionChanged?.Invoke(currentSelection);
            return;
        }

        ClearSelectionInternal(notify: false);

        currentSelection = selectable;
        selectedUnits.Add(selectable);
        selectable.Select();

        SelectionChanged?.Invoke(currentSelection);
    }

    public void ClearSelection()
    {
        if (currentSelection == null && selectedUnits.Count == 0)
            return;

        ClearSelectionInternal(notify: true);
    }

    private void ClearSelectionInternal(bool notify)
    {
        foreach (var unit in selectedUnits)
        {
            if (unit != null)
                unit.Deselect();
        }

        selectedUnits.Clear();
        currentSelection = null;

        if (notify)
            SelectionCleared?.Invoke();
    }

    public IReadOnlyList<UnitSelectable> GetSelectedUnits()
    {
        return selectedUnits;
    }

    public UnitSelectable GetCurrentSelection()
    {
        return currentSelection;
    }
}

