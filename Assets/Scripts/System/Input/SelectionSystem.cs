using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// <summary>
/// Отвечает за выбор игровых объектов игроком через touch input.
/// 
/// Основные задачи:
/// - обработать тап по экрану
/// - определить, попал ли игрок по selectable-объекту
/// - выделить найденный объект
/// - снять выделение при тапе в пустое место
/// - уведомить другие системы о смене выбора через события
/// </summary>
public class SelectionSystem : MonoBehaviour
{
    public event Action<UnitSelectable> SelectionChanged;  // Передаёт текущий выбранный UnitSelectable.
    public event Action SelectionCleared;  // Вызывается, когда выделение полностью очищено.

    [Header("Raycast")]
    [SerializeField] private LayerMask ignoreRaycastLayer;

    private Camera mainCamera;
    private UnitSelectable currentSelection;     // Текущий выбранный объект.
    private readonly List<UnitSelectable> selectedUnits = new();      // Список выделенных объектов.

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
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleTouch();
    }

    /// <summary>
    /// Проверяет активное касание и запускает выбор объекта,
    /// если пользователь закончил тап по игровому миру.
    /// </summary>
    private void HandleTouch()
    {
        if (Touch.activeTouches.Count == 0)
            return;

        var touch = Touch.activeTouches[0];

        if (touch.phase != TouchPhase.Ended)
            return;

        // Если тап был по UI, игровой выбор не обрабатываем.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.touchId))
            return;

        ProcessTap(touch.screenPosition);
    }

    /// <summary>
    /// Обрабатывает тап по экрану:
    /// переводит координаты в мир,
    /// ищет collider,
    /// пытается получить UnitSelectable
    /// и либо выбирает объект, либо очищает текущее выделение.
    /// </summary>
    private void ProcessTap(Vector2 screenPos)
    {
        if (mainCamera == null)
        {
            return;
        }

        Vector3 worldPos3 = mainCamera.ScreenToWorldPoint(screenPos);
        Vector2 worldPos = new(worldPos3.x, worldPos3.y);

        // Инвертирует маску, чтобы исключить ignoreRaycastLayer из проверки.
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
    /// Позволяет UI выбрать рабочего через ссылку на Worker.
    /// Используется, например, при клике по элементу списка рабочих.
    /// </summary>
    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
            return;

        UnitSelectable selectable = worker.GetComponentInParent<UnitSelectable>();

        if (selectable == null)
        {
            return;
        }

        Select(selectable);
    }

    /// <summary>
    /// Выбирает переданный объект
    /// Если объект уже выбран, повторно отправляет событие SelectionChanged,
    /// чтобы UI или другие системы могли обновиться.
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


    /// <summary>
    /// Внутренний метод очистки выделения.
    /// notify = true:
    ///     отправляет событие SelectionCleared
    /// notify = false:
    ///     просто очищает состояние без уведомления
    /// </summary>
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

