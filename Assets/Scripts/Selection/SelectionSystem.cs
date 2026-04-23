using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// Система выбора объектов игроком.
/// Поддерживает:
/// - выбор одного worker или здания
/// - добавление нескольких союзных боевых юнитов в общую группу
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
    /// Пытается восстановить ссылку на главную камеру.
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

        if (TouchUtility.IsPointerOverUI(touch))
            return;

        ProcessTap(touch.screenPosition);
    }

    /// <summary>
    /// Выполняет raycast по месту тапа и решает, что выбрать.
    /// </summary>
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

    /// <summary>
    /// Выбор worker'а из UI.
    /// Для worker всегда сбрасываем текущее выделение и выбираем только его.
    /// </summary>
    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
            return;

        UnitSelectable selectable = worker.GetComponentInParent<UnitSelectable>();
        if (selectable == null)
            return;

        ClearSelectionInternal(notify: false);
        AddSingleSelection(selectable);
        SelectionChanged?.Invoke(currentSelection);
    }

    /// <summary>
    /// Основная логика выбора объекта.
    /// </summary>
    public void Select(UnitSelectable selectable)
    {
        if (selectable == null)
            return;

        bool isArmyUnit = IsPlayerArmyUnit(selectable);

        if (isArmyUnit)
        {
            AddArmySelection(selectable);
            SelectionChanged?.Invoke(currentSelection);
            return;
        }

        // Worker / здание / другой не-боевой объект:
        // очищаем старое и выбираем только его.
        ClearSelectionInternal(notify: false);
        AddSingleSelection(selectable);
        SelectionChanged?.Invoke(currentSelection);
    }

    /// <summary>
    /// Добавляет боевого юнита игрока в текущее выделение.
    /// Уже выбранный юнит повторно не добавляется.
    /// </summary>
    private void AddArmySelection(UnitSelectable selectable)
    {
        if (selectedUnits.Contains(selectable))
        {
            currentSelection = selectable;
            return;
        }

        // Если в выделении сейчас есть не-боевые объекты,
        // сначала очищаем группу.
        if (ContainsNonArmySelection())
            ClearSelectionInternal(notify: false);

        selectedUnits.Add(selectable);
        selectable.Select();
        currentSelection = selectable;
    }

    /// <summary>
    /// Выбирает один объект, полностью заменяя текущее выделение.
    /// </summary>
    private void AddSingleSelection(UnitSelectable selectable)
    {
        selectedUnits.Add(selectable);
        selectable.Select();
        currentSelection = selectable;
    }

    /// <summary>
    /// Проверяет, есть ли в текущем выделении объекты, которые не являются боевыми юнитами игрока.
    /// </summary>
    private bool ContainsNonArmySelection()
    {
        foreach (UnitSelectable unit in selectedUnits)
        {
            if (unit == null)
                continue;

            if (!IsPlayerArmyUnit(unit))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Проверяет, является ли selectable боевым юнитом игрока.
    /// </summary>
    private bool IsPlayerArmyUnit(UnitSelectable selectable)
    {
        if (selectable == null)
            return false;

        ArmyUnit armyUnit = selectable.GetComponent<ArmyUnit>();
        if (armyUnit == null)
            armyUnit = selectable.GetComponentInParent<ArmyUnit>();

        return armyUnit != null && armyUnit.IsPlayerUnit();
    }

    public void ClearSelection()
    {
        if (currentSelection == null && selectedUnits.Count == 0)
            return;

        ClearSelectionInternal(notify: true);
    }

    private void ClearSelectionInternal(bool notify)
    {
        foreach (UnitSelectable unit in selectedUnits)
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
    /// <summary>
    /// Полностью заменяет текущее выделение на указанный список боевых юнитов.
    /// Используется, например, кнопкой "Выбрать всех".
    /// </summary>
    public void SelectArmyUnits(IReadOnlyList<ArmyUnit> armyUnits)
    {
        ClearSelectionInternal(notify: false);

        if (armyUnits == null)
        {
            SelectionCleared?.Invoke();
            return;
        }

        foreach (ArmyUnit armyUnit in armyUnits)
        {
            if (armyUnit == null || !armyUnit.IsPlayerUnit())
                continue;

            UnitSelectable selectable = armyUnit.GetComponent<UnitSelectable>();
            if (selectable == null)
                selectable = armyUnit.GetComponentInParent<UnitSelectable>();

            if (selectable == null)
                continue;

            if (selectedUnits.Contains(selectable))
                continue;

            selectedUnits.Add(selectable);
            selectable.Select();
            currentSelection = selectable;
        }

        if (selectedUnits.Count == 0)
            SelectionCleared?.Invoke();
        else
            SelectionChanged?.Invoke(currentSelection);
    }
}