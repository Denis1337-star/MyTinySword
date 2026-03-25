using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class SelectionSystem : MonoBehaviour
{
    public event Action<UnitSelectable> SelectionChanged;
    public event Action SelectionCleared;

    [Header("Raycast")]
    [SerializeField] private LayerMask ignoreRaycastLayer;

    [Header("References")]
    [SerializeField] private Camera mainCamera;

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
        ResolveReferences();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (mainCamera == null)
        {
            if (GameServices.Instance != null && GameServices.Instance.MainCamera != null)
                mainCamera = GameServices.Instance.MainCamera;
            else
                mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
        if (!TouchUtility.TryGetEndedTouch(out var touch))
            return;

        if (TouchUtility.IsPointerOverUI(touch))
            return;

        ProcessTap(touch.screenPosition);
    }

    private void ProcessTap(Vector2 screenPos)
    {
        if (mainCamera == null)
            return;

        Vector2 worldPos = TouchUtility.ScreenToWorld(mainCamera, screenPos);

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
            return;

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