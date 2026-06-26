using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI представление одной ноды дерева развития.
/// Показывает состояние ноды и сообщает наружу о клике.
/// </summary>
public sealed class TechTreeNodeView : ValidatedMonoBehaviour
{
    [Header("Config")]
    [SerializeField] private TechTreeNodeConfig _config;

    [Header("Click")]
    [SerializeField] private Button _button;

    [Header("Node Visual")]
    [SerializeField] private Image _nodeImage;
    [SerializeField] private GameObject _selectionOutline;

    [Header("Access Icon")]
    [SerializeField] private Image _accessIcon;
    [SerializeField] private Sprite _lockedSprite;
    [SerializeField] private Sprite _maxedSprite;

    [Header("Upgrade Timer")]
    [SerializeField] private GameObject _clockIcon;
    [SerializeField] private TMP_Text _nodeTimerText;

    [Header("Text")]
    [SerializeField] private TMP_Text _levelText;

    [Header("Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _availableColor = new(1f, 0.9f, 0.35f, 1f);
    [SerializeField] private Color _upgradingColor = new(0.35f, 0.75f, 1f, 1f);
    [SerializeField] private Color _maxedColor = new(0.35f, 1f, 0.35f, 1f);

    private Action<TechTreeNodeView> _onClicked;

    public TechTreeNodeConfig Config => _config;

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsValidConfig(this, _config, nameof(_config));
        valid &= ValidationUtility.IsAssigned(this, _button, nameof(_button));
        valid &= ValidationUtility.IsAssigned(this, _nodeImage, nameof(_nodeImage));
        valid &= ValidationUtility.IsAssigned(this, _levelText, nameof(_levelText));
        valid &= ValidationUtility.IsAssigned(this, _nodeTimerText, nameof(_nodeTimerText));

        return valid;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(HandleClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(HandleClicked);
    }

    public void Initialize(Action<TechTreeNodeView> onClicked)
    {
        _onClicked = onClicked;
    }

    public void Refresh(
        TechTreeNodeSaveData saveData,
        TechTreeNodeState state,
        bool selected,
        long remainingSeconds)
    {
        int level = saveData != null ? saveData.Level : 0;
        int maxLevel = _config.MaxLevel;

        _levelText.text = $"{level}/{maxLevel}";

        RefreshSelection(selected);
        RefreshNodeColor(state);
        RefreshAccessIcon(state);
        RefreshTimer(state, remainingSeconds);
    }

    private void RefreshSelection(bool selected)
    {
        SetOptionalView(_selectionOutline, selected);
    }

    private void RefreshNodeColor(TechTreeNodeState state)
    {
        _nodeImage.color = state switch
        {
            TechTreeNodeState.Available => _availableColor,
            TechTreeNodeState.Upgrading => _upgradingColor,
            TechTreeNodeState.Maxed => _maxedColor,
            _ => _normalColor
        };
    }

    private void RefreshAccessIcon(TechTreeNodeState state)
    {
        if (_accessIcon == null)
            return;

        if (state == TechTreeNodeState.Locked)
        {
            _accessIcon.gameObject.SetActive(true);
            _accessIcon.sprite = _lockedSprite;
            return;
        }

        if (state == TechTreeNodeState.Maxed)
        {
            _accessIcon.gameObject.SetActive(true);
            _accessIcon.sprite = _maxedSprite;
            return;
        }

        _accessIcon.gameObject.SetActive(false);
    }

    private void RefreshTimer(TechTreeNodeState state, long remainingSeconds)
    {
        bool upgrading = state == TechTreeNodeState.Upgrading;

        SetOptionalView(_clockIcon, upgrading);

        _nodeTimerText.gameObject.SetActive(upgrading);

        if (upgrading)
            _nodeTimerText.text = FormatTime(remainingSeconds);
    }

    private void HandleClicked()
    {
        _onClicked?.Invoke(this);
    }

    private static void SetOptionalView(GameObject view, bool active)
    {
        if (view == null)
            return;

        view.SetActive(active);
    }

    public static string FormatTime(long seconds)
    {
        if (seconds < 0)
            seconds = 0;

        long minutes = seconds / 60;
        long remainingSeconds = seconds % 60;

        return $"{minutes:00}:{remainingSeconds:00}";
    }
}