using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

/// <summary>
/// —павнит и обновл€ет HP bar над объектом
/// </summary>
public sealed class HealthBarSpawner : ValidatedMonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private FactionMember _factionMember;
    [SerializeField] private UnitSelectable _selectable;
    [SerializeField] private WorldHealthBarAnchor _anchor;
    [SerializeField] private HealthBarView _healthBarPrefab;
    [SerializeField] private bool _showWhenSelected = true;
    [SerializeField] private bool _showWhenDamaged = true;
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

    protected override void Awake()
    {
        ResolveReferences();

        base.Awake();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
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

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _health, nameof(_health));
        valid &= ValidationUtility.IsAssigned(this, _healthBarPrefab, nameof(_healthBarPrefab));

        return valid;
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
        _damagedOnce = current < max;

        if (!ShouldShowBar())
        {
            HideBar();
            return;
        }

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

        if (_screenCanvas == null || _mainCamera == null)
        {
            Debug.LogWarning($"{name}: HPBar не может по€витьс€ Ч нет Canvas или Camera.", this);
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

        if (_factionMember == null)
            _factionMember = GetComponent<FactionMember>();

        if (_selectable == null)
            _selectable = GetComponent<UnitSelectable>();

        if (_anchor == null)
            _anchor = GetComponentInChildren<WorldHealthBarAnchor>();
    }
}