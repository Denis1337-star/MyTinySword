using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

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

    private void HandleMoveCommand()
    {
        if (!TouchUtility.TryGetEndedTouch(out var touch))
            return;

        if (TouchUtility.IsPointerOverUI(touch))
            return;

        if (selectionSystem == null || mainCamera == null)
            return;

        Vector2 worldPos = TouchUtility.ScreenToWorld(mainCamera, touch.screenPosition);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null)
        {
            UnitSelectable selectable = hit.collider.GetComponentInParent<UnitSelectable>();
            if (selectable != null)
                return;
        }

        IssueMoveCommand(worldPos);
    }

    private void IssueMoveCommand(Vector2 targetPos)
    {
        var selectedUnits = selectionSystem.GetSelectedUnits();
        if (selectedUnits.Count == 0)
            return;

        foreach (var selectable in selectedUnits)
        {
            if (selectable == null)
                continue;

            if (selectable.TryGetComponent(out Worker worker))
                continue;

            UnitMovement movement = selectable.GetComponent<UnitMovement>();
            if (movement != null)
                movement.MoveTo(targetPos);
        }
    }
}