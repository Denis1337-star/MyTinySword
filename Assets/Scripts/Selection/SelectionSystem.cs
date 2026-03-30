using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// Система выбора объектов игроком.
/// Отвечает за обработку тапа по миру, выбор selectable-объекта,
/// снятие предыдущего выделения и уведомление подписчиков об изменениях selection.
/// </summary>
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
    /// <summary>
    /// Пытается восстановить ссылку на главную камеру
    /// Сначала через GameServices, потом через Camera.main
    /// </summary>
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
    /// <summary>
    /// Обрабатывает пользовательский тап для выбора объекта.
    /// </summary>
    private void HandleTouch()
    {
        if (!TouchUtility.TryGetEndedTouch(out var touch))
            return;

        if (TouchUtility.IsPointerOverUI(touch))   // Нажатия по UI не должны менять игровой selection
            return;

        ProcessTap(touch.screenPosition);
    }
    /// <summary>
    /// Выполняет raycast в точку тапа и решает,
    /// нужно ли выбрать объект или очистить selection
    /// </summary>
    private void ProcessTap(Vector2 screenPos)
    {
        if (mainCamera == null)
            return;

        // Переводим координату тапа из экрана в игровой мир
        Vector2 worldPos = TouchUtility.ScreenToWorld(mainCamera, screenPos);

        // Исключаем из raycast слои, которые не должны участвовать в выборе
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

    /// <summary>
    /// Выбирает worker'а по запросу из UI
    /// Используется, например, при клике по элементу в списке worker'ов
    /// </summary>
    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
            return;

        UnitSelectable selectable = worker.GetComponentInParent<UnitSelectable>();
        if (selectable == null)
            return;

        Select(selectable);
    }
    /// <summary>
    /// Делает указанный объект текущим выбранным
    /// </summary>
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