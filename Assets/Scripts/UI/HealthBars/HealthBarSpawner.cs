using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Управляет появлением и обновлением HP bar над объектом
/// Для юнитов:
/// - показывается только при получении урона
/// Для зданий:
/// - показывается при получении урона и при выборе
/// </summary>
public class HealthBarSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private FactionMember factionMember;
    [SerializeField] private UnitSelectable selectable;
    [SerializeField] private WorldHealthBarAnchor anchor;

    [Header("UI")]
    [SerializeField] private HealthBarView healthBarPrefab;
    [SerializeField] private Canvas screenCanvas;

    [Header("Type")]
    [SerializeField] private bool isBuilding = false;

    [Header("Offset Fallback")]
    [SerializeField] private Vector3 fallbackOffset = new Vector3(0f, 1.5f, 0f);

    private HealthBarView spawnedBar;
    private Camera mainCamera;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();

        if (factionMember == null)
            factionMember = GetComponent<FactionMember>();

        if (selectable == null)
            selectable = GetComponent<UnitSelectable>();

        if (anchor == null)
            anchor = GetComponentInChildren<WorldHealthBarAnchor>();

        if (screenCanvas == null)
            screenCanvas = FindObjectOfType<Canvas>(true);

        if (GameServices.Instance != null && GameServices.Instance.MainCamera != null)
            mainCamera = GameServices.Instance.MainCamera;
        else
            mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnHealthChanged += OnHealthChanged;
            health.OnDied += OnDied;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged -= OnHealthChanged;
            health.OnDied -= OnDied;
        }

        DestroyBar();
    }

    private void Update()
    {
        if (!isBuilding)
            return;

        if (selectable == null || health == null || health.IsDead)
            return;

        if (selectable.IsSelected)
        {
            ShowBar();
            RefreshBar();
        }
        else if (!WasDamaged())
        {
            HideBar();
        }
    }

    private bool damagedOnce;

    private void OnHealthChanged(int current, int max)
    {
        damagedOnce = true;

        ShowBar();
        RefreshBar();

        if (!isBuilding && current >= max)
            HideBar();
    }

    private void OnDied()
    {
        HideBar();
    }

    private void ShowBar()
    {
        if (spawnedBar != null)
            return;

        if (healthBarPrefab == null || screenCanvas == null || mainCamera == null)
            return;

        spawnedBar = Instantiate(healthBarPrefab, screenCanvas.transform);

        Color color = GetBarColor();
        Vector3 offset = anchor != null
            ? anchor.transform.position - transform.position
            : fallbackOffset;

        Transform targetTransform = anchor != null ? anchor.transform : transform;
        spawnedBar.Initialize(targetTransform, mainCamera, color, offset);
    }

    private void RefreshBar()
    {
        if (spawnedBar == null || health == null)
            return;

        float normalized = health.MaxHealth > 0
            ? (float)health.CurrentHealth / health.MaxHealth
            : 0f;

        spawnedBar.SetFill(normalized);
    }

    private void HideBar()
    {
        DestroyBar();
    }

    private void DestroyBar()
    {
        if (spawnedBar != null)
            Destroy(spawnedBar.gameObject);

        spawnedBar = null;
    }

    private bool WasDamaged()
    {
        return damagedOnce && health != null && health.CurrentHealth < health.MaxHealth;
    }

    private Color GetBarColor()
    {
        if (factionMember == null)
            return Color.green;

        return factionMember.Faction == FactionType.Enemy
            ? Color.red
            : Color.green;
    }
}
