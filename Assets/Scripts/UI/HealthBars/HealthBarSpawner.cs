using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

/// <summary>
/// Спавнит и обновляет HP bar над объектом.
/// Работает для воинов, worker'ов, зданий и врагов.
/// </summary>
public sealed class HealthBarSpawner : MonoBehaviour
{
    [Header("References")]
    [FormerlySerializedAs("health")]
    [SerializeField] private Health _health;

    [FormerlySerializedAs("factionMember")]
    [SerializeField] private FactionMember _factionMember;

    [FormerlySerializedAs("selectable")]
    [SerializeField] private UnitSelectable _selectable;

    [FormerlySerializedAs("anchor")]
    [SerializeField] private WorldHealthBarAnchor _anchor;

    [Header("UI")]
    [FormerlySerializedAs("healthBarPrefab")]
    [SerializeField] private HealthBarView _healthBarPrefab;

    [Header("Behaviour")]
    [SerializeField] private bool _showWhenSelected = true;
    [SerializeField] private bool _showWhenDamaged = true;

    [Header("Offset Fallback")]
    [FormerlySerializedAs("fallbackOffset")]
    [SerializeField] private Vector3 _fallbackOffset = new(0f, 1.5f, 0f);

    private HealthBarView _spawnedBar;
    private Canvas _screenCanvas;
    private Camera _mainCamera;

    private bool _damagedOnce;

    [Inject]
    private void Construct(
        Camera mainCamera,
        Canvas screenCanvas)
    {
        _mainCamera = mainCamera;
        _screenCanvas = screenCanvas;
    }

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        if (_health == null)
            return;

        _health.OnHealthChanged += OnHealthChanged;
        _health.OnDied += OnDied;
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnHealthChanged -= OnHealthChanged;
            _health.OnDied -= OnDied;
        }

        DestroyBar();
    }

    private void Update()
    {
        if (_health == null || _health.IsDead)
            return;

        bool shouldShow = ShouldShowBar();

        if (shouldShow)
        {
            ShowBar();
            RefreshBar();
            return;
        }

        HideBar();
    }

    private void OnHealthChanged(int current, int max)
    {
        _damagedOnce = true;

        ShowBar();
        RefreshBar();
    }

    private void OnDied()
    {
        HideBar();
    }

    private bool ShouldShowBar()
    {
        bool selected = _showWhenSelected &&
                        _selectable != null &&
                        _selectable.IsSelected;

        bool damaged = _showWhenDamaged &&
                       _damagedOnce &&
                       _health != null &&
                       _health.CurrentHealth < _health.MaxHealth;

        return selected || damaged;
    }

    private void ShowBar()
    {
        if (_spawnedBar != null)
            return;

        if (_healthBarPrefab == null)
            return;

        if (_screenCanvas == null)
            _screenCanvas = FindObjectOfType<Canvas>();

        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_screenCanvas == null || _mainCamera == null)
        {
            Debug.LogWarning($"{name}: HPBar не может появиться — нет Canvas или Camera.", this);
            return;
        }

        _spawnedBar = Instantiate(_healthBarPrefab, _screenCanvas.transform);

        Color color = GetBarColor();

        Transform targetTransform;
        Vector3 offset;

        if (_anchor != null)
        {
            targetTransform = _anchor.transform;
            offset = Vector3.zero;
        }
        else
        {
            targetTransform = transform;
            offset = _fallbackOffset;
        }

        _spawnedBar.Initialize(
            targetTransform,
            _mainCamera,
            _screenCanvas,
            color,
            offset);
    }

    private void RefreshBar()
    {
        if (_spawnedBar == null || _health == null)
            return;

        float normalized = _health.MaxHealth > 0
            ? (float)_health.CurrentHealth / _health.MaxHealth
            : 0f;

        _spawnedBar.SetFill(normalized);
    }

    private void HideBar()
    {
        DestroyBar();
    }

    private void DestroyBar()
    {
        if (_spawnedBar != null)
            Destroy(_spawnedBar.gameObject);

        _spawnedBar = null;
    }

    private Color GetBarColor()
    {
        if (_factionMember == null)
            return Color.green;

        return _factionMember.Faction == FactionType.Enemy
            ? Color.red
            : Color.green;
    }

    private void ResolveReferences()
    {
        if (_health == null)
            _health = GetComponent<Health>();

        if (_health == null)
            _health = GetComponentInParent<Health>();

        if (_factionMember == null)
            _factionMember = GetComponent<FactionMember>();

        if (_factionMember == null)
            _factionMember = GetComponentInParent<FactionMember>();

        if (_selectable == null)
            _selectable = GetComponent<UnitSelectable>();

        if (_selectable == null)
            _selectable = GetComponentInParent<UnitSelectable>();

        if (_anchor == null)
            _anchor = GetComponentInChildren<WorldHealthBarAnchor>();
    }
}