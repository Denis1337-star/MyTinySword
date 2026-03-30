using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// Система пользовательских команд
/// Отвечает за обработку тапа по миру и отдачу команды движения
/// выбранным юнитам, которые поддерживают ручное перемещение
/// </summary>
public class CommandSystem : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }

    /// <summary>
    /// Пытается заполнить отсутствующие ссылки через GameServices
    /// или через прямой поиск по сцене
    /// </summary>
    private void ResolveReferences()
    {
        if (selectionSystem == null)
        {
            if (GameServices.Instance != null && GameServices.Instance.Selection != null)
                selectionSystem = GameServices.Instance.Selection;
            else
                selectionSystem = FindObjectOfType<SelectionSystem>(true);
        }

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
        HandleMoveCommand();
    }
    /// <summary>
    /// Проверяет, была ли отдана команда движения через тап по миру
    /// </summary>
    private void HandleMoveCommand()
    {
        if (!TouchUtility.TryGetEndedTouch(out var touch))
            return;

        if (TouchUtility.IsPointerOverUI(touch))  // Если тап был по UI — игровой мир не обрабатываем
            return;

        if (selectionSystem == null || mainCamera == null)
            return;

        Vector2 worldPos = TouchUtility.ScreenToWorld(mainCamera, touch.screenPosition);

        // Если тап пришёлся по selectable-объекту,
        // не считаем это командой движения — этот тап относится к выбору
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null)
        {
            UnitSelectable selectable = hit.collider.GetComponentInParent<UnitSelectable>();
            if (selectable != null)
                return;
        }

        IssueMoveCommand(worldPos);
    }
    /// <summary>
    /// Отдаёт команду движения всем выбранным объектам
    /// которые поддерживают ручное перемещение
    /// </summary>
    private void IssueMoveCommand(Vector2 targetPos)
    {
        var selectedUnits = selectionSystem.GetSelectedUnits();
        if (selectedUnits.Count == 0)
            return;

        foreach (var selectable in selectedUnits)
        {
            if (selectable == null)
                continue;

            // Worker'ов вручную не двигаем
            // они управляются своей AI/state machine логикой
            if (selectable.TryGetComponent(out Worker worker))
                continue;

            UnitMovement movement = selectable.GetComponent<UnitMovement>();
            if (movement != null)
                movement.MoveTo(targetPos);
        }
    }
}