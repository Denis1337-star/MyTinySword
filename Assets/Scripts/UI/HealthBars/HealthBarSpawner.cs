using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

/// <summary>
/// Управляет появлением и обновлением HP bar над объектом
/// </summary>
public class HealthBarSpawner : MonoBehaviour
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

    [Header("Type")]
    [FormerlySerializedAs("isBuilding")]
    [SerializeField] private bool _isBuilding;

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
        if (!_isBuilding)
            return;

        if (_selectable == null || _health == null || _health.IsDead)
            return;

        if (_selectable.IsSelected)
        {
            ShowBar();
            RefreshBar();
            return;
        }

        if (!WasDamaged())
            HideBar();
    }

    private void OnHealthChanged(int current, int max)
    {
        _damagedOnce = true;

        ShowBar();
        RefreshBar();

        if (!_isBuilding && current >= max)
            HideBar();
    }

    private void OnDied()
    {
        HideBar();
    }

    private void ShowBar()
    {
        if (_spawnedBar != null)
            return;

        if (_healthBarPrefab == null || _screenCanvas == null || _mainCamera == null)
            return;

        _spawnedBar = Instantiate(_healthBarPrefab, _screenCanvas.transform);

        Color color = GetBarColor();

        Vector3 offset = _anchor != null
            ? _anchor.transform.position - transform.position
            : _fallbackOffset;

        Transform targetTransform = _anchor != null
            ? _anchor.transform
            : transform;

        _spawnedBar.Initialize(targetTransform, _mainCamera, color, offset);
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

    private bool WasDamaged()
    {
        return _damagedOnce &&
               _health != null &&
               _health.CurrentHealth < _health.MaxHealth;
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
