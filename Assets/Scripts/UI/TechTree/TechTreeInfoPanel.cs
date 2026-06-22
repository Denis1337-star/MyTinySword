using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Панель подробной информации о выбранной ноде дерева развития.
/// Переключает три состояния: доступна/улучшается, заблокирована, максимум.
/// </summary>
public sealed class TechTreeInfoPanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;
    [SerializeField] private SimplePanelTween _panelTween;

    [Header("Header")]
    [SerializeField] private Image _nodeIconImage;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;

    [Header("State Blocks")]
    [SerializeField] private GameObject _availableBlock;
    [SerializeField] private GameObject _lockedBlock;
    [SerializeField] private GameObject _maxLevelBlock;

    [Header("Available Block")]
    [SerializeField] private TMP_Text _currentLevelText;
    [SerializeField] private TMP_Text _nextLevelText;
    [SerializeField] private TMP_Text _bonusPreviewText;
    [SerializeField] private TMP_Text _upgradeTimeText;
    [SerializeField] private Image _progressFillImage;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TMP_Text _upgradeButtonText;

    [Header("Locked Block")]
    [SerializeField] private TMP_Text _requirementTitleText;
    [SerializeField] private TMP_Text _requirementListText;

    [Header("Max Level Block")]
    [SerializeField] private TMP_Text _maxCurrentLevelText;
    [SerializeField] private TMP_Text _maxBonusText;

    public event Action UpgradeClicked;

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _titleText, nameof(_titleText));
        valid &= ValidationUtility.IsAssigned(this, _descriptionText, nameof(_descriptionText));

        valid &= ValidationUtility.IsAssigned(this, _availableBlock, nameof(_availableBlock));
        valid &= ValidationUtility.IsAssigned(this, _lockedBlock, nameof(_lockedBlock));
        valid &= ValidationUtility.IsAssigned(this, _maxLevelBlock, nameof(_maxLevelBlock));

        valid &= ValidationUtility.IsAssigned(this, _currentLevelText, nameof(_currentLevelText));
        valid &= ValidationUtility.IsAssigned(this, _nextLevelText, nameof(_nextLevelText));
        valid &= ValidationUtility.IsAssigned(this, _bonusPreviewText, nameof(_bonusPreviewText));
        valid &= ValidationUtility.IsAssigned(this, _upgradeTimeText, nameof(_upgradeTimeText));
        valid &= ValidationUtility.IsAssigned(this, _upgradeButton, nameof(_upgradeButton));
        valid &= ValidationUtility.IsAssigned(this, _upgradeButtonText, nameof(_upgradeButtonText));

        valid &= ValidationUtility.IsAssigned(this, _requirementTitleText, nameof(_requirementTitleText));
        valid &= ValidationUtility.IsAssigned(this, _requirementListText, nameof(_requirementListText));

        valid &= ValidationUtility.IsAssigned(this, _maxCurrentLevelText, nameof(_maxCurrentLevelText));
        valid &= ValidationUtility.IsAssigned(this, _maxBonusText, nameof(_maxBonusText));

        return valid;
    }

    private void OnEnable()
    {
        _upgradeButton.onClick.AddListener(HandleUpgradeClicked);
    }

    private void OnDisable()
    {
        _upgradeButton.onClick.RemoveListener(HandleUpgradeClicked);
    }

    public void Show()
    {
        if (_panelTween != null)
        {
            _panelTween.Show();
            return;
        }

        _root.SetActive(true);
    }

    public void Hide()
    {
        if (_panelTween != null)
        {
            _panelTween.Hide();
            return;
        }

        _root.SetActive(false);
    }

    public void HideImmediate()
    {
        if (_panelTween != null)
        {
            _panelTween.HideImmediate();
            return;
        }

        _root.SetActive(false);
    }

    public void Refresh(
        TechTreeNodeConfig config,
        TechTreeNodeSaveData saveData,
        TechTreeNodeState state,
        string requirementsText,
        long remainingSeconds,
        bool anotherNodeUpgrading)
    {
        int level = saveData != null ? saveData.Level : 0;

        RefreshHeader(config);

        switch (state)
        {
            case TechTreeNodeState.Locked:
                RefreshLocked(requirementsText);
                break;

            case TechTreeNodeState.Maxed:
                RefreshMaxLevel(config, level);
                break;

            default:
                RefreshAvailable(config, level, state, remainingSeconds, anotherNodeUpgrading);
                break;
        }
    }

    private void RefreshHeader(TechTreeNodeConfig config)
    {
        if (_nodeIconImage != null)
            _nodeIconImage.sprite = config.Icon;

        _titleText.text = config.DisplayName;
        _descriptionText.text = config.Description;
    }

    private void RefreshAvailable(
        TechTreeNodeConfig config,
        int level,
        TechTreeNodeState state,
        long remainingSeconds,
        bool anotherNodeUpgrading)
    {
        SetStateBlocks(available: true, locked: false, maxLevel: false);

        int nextLevel = Mathf.Min(level + 1, config.MaxLevel);

        _currentLevelText.text = $"{level}\\{config.MaxLevel}";
        _nextLevelText.text = $"{nextLevel}\\{config.MaxLevel}";
        _bonusPreviewText.text = $"{config.GetBonusText(level)} -> {config.GetBonusText(nextLevel)}";

        int upgradeSeconds = config.GetUpgradeSeconds(level);
        _upgradeTimeText.text = TechTreeNodeView.FormatTime(
            state == TechTreeNodeState.Upgrading
                ? remainingSeconds
                : upgradeSeconds);

        RefreshProgress(state, remainingSeconds, upgradeSeconds);
        RefreshUpgradeButton(state, anotherNodeUpgrading);
    }

    private void RefreshLocked(string requirementsText)
    {
        SetStateBlocks(available: false, locked: true, maxLevel: false);

        _requirementTitleText.text = "Надо прокачать";
        _requirementListText.text = requirementsText;
    }

    private void RefreshMaxLevel(TechTreeNodeConfig config, int level)
    {
        SetStateBlocks(available: false, locked: false, maxLevel: true);

        _maxCurrentLevelText.text = $"{level}\\{config.MaxLevel}";
        _maxBonusText.text = config.GetBonusText(level);
    }

    private void RefreshProgress(
        TechTreeNodeState state,
        long remainingSeconds,
        int upgradeSeconds)
    {
        if (_progressFillImage == null)
            return;

        if (state != TechTreeNodeState.Upgrading || upgradeSeconds <= 0)
        {
            _progressFillImage.fillAmount = 0f;
            return;
        }

        float normalizedRemaining = (float)remainingSeconds / upgradeSeconds;
        _progressFillImage.fillAmount = 1f - Mathf.Clamp01(normalizedRemaining);
    }

    private void RefreshUpgradeButton(
        TechTreeNodeState state,
        bool anotherNodeUpgrading)
    {
        bool canUpgrade =
            (state == TechTreeNodeState.Available || state == TechTreeNodeState.Selected) &&
            !anotherNodeUpgrading;

        _upgradeButton.interactable = canUpgrade;

        if (state == TechTreeNodeState.Upgrading)
        {
            _upgradeButtonText.text = "Улучшается";
            return;
        }

        if (anotherNodeUpgrading)
        {
            _upgradeButtonText.text = "Уже идёт улучшение";
            return;
        }

        _upgradeButtonText.text = "Улучшить";
    }

    private void SetStateBlocks(bool available, bool locked, bool maxLevel)
    {
        _availableBlock.SetActive(available);
        _lockedBlock.SetActive(locked);
        _maxLevelBlock.SetActive(maxLevel);
    }

    private void HandleUpgradeClicked()
    {
        UpgradeClicked?.Invoke();
    }
}