using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class SelectionSystem : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private LayerMask ignoredLayers;

    private Camera _camera;
    private readonly List<UnitSelectable> _selected = new();

    public UnitSelectable CurrentSelection { get; private set; }
    public IReadOnlyList<UnitSelectable> SelectedUnits => _selected;

    public event Action<UnitSelectable> SelectionChanged;
    public event Action SelectionCleared;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
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

        TrySelectAtScreenPosition(touch.screenPosition);
    }

    private void TrySelectAtScreenPosition(Vector2 screenPosition)
    {
        if (_camera == null)
            return;

        Vector3 world3 = _camera.ScreenToWorldPoint(screenPosition);
        Vector2 world = new(world3.x, world3.y);

        int mask = ~ignoredLayers.value;
        RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero, 100f, mask);

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

    public void Select(UnitSelectable selectable)
    {
        if (selectable == null)
            return;

        if (CurrentSelection == selectable)
            return;

        ClearSelectionInternal();

        CurrentSelection = selectable;
        _selected.Add(selectable);
        selectable.Select();

        SelectionChanged?.Invoke(selectable);
    }

    public void ClearSelection()
    {
        if (CurrentSelection == null && _selected.Count == 0)
            return;

        ClearSelectionInternal();
        SelectionCleared?.Invoke();
    }

    private void ClearSelectionInternal()
    {
        foreach (var selectable in _selected)
        {
            if (selectable != null)
                selectable.Deselect();
        }

        _selected.Clear();
        CurrentSelection = null;
    }

    // =========================
    // Temporary compatibility API
    // =========================

    public IReadOnlyList<UnitSelectable> GetSelectedUnits()
    {
        return _selected;
    }

    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
            return;

        UnitSelectable selectable = worker.GetComponent<UnitSelectable>();
        if (selectable == null)
            selectable = worker.GetComponentInParent<UnitSelectable>();

        if (selectable != null)
            Select(selectable);
    }

    public void ShowWorkerUI(Worker worker)
    {
        // compatibility shim
        // UI is now handled by SelectionUiPresenter
    }

    public void ShowHouseUI(House house)
    {
        // compatibility shim
        // UI is now handled by SelectionUiPresenter
    }
}

