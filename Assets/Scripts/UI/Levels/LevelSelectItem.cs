using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Один элемент выбора уровня в меню.
/// Показывает название уровня, статус открытия/прохождения
/// и сообщает наружу, когда игрок нажал на открытый уровень.
/// </summary>
public sealed class LevelSelectItem : ValidatedMonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _statusText;

    [Header("Optional Visuals")]
    [SerializeField] private GameObject _lockedView;
    [SerializeField] private GameObject _completedView;

    private LevelConfig _levelConfig;
    private Action<LevelConfig> _onClicked;

    private bool _unlocked;

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _button, nameof(_button));
        valid &= ValidationUtility.IsAssigned(this, _titleText, nameof(_titleText));
        valid &= ValidationUtility.IsAssigned(this, _statusText, nameof(_statusText));

        return valid;
    }

    public void Initialize(
        LevelConfig levelConfig,
        bool unlocked,
        bool completed,
        Action<LevelConfig> onClicked)
    {
        _levelConfig = levelConfig;
        _unlocked = unlocked;
        _onClicked = onClicked;

        RefreshView(completed);
    }

    private void RefreshView(bool completed)
    {
        if (_levelConfig == null)
        {
            _button.interactable = false;
            _titleText.text = "Уровень не задан";
            _statusText.text = "Ошибка";
            SetOptionalView(_lockedView, true);
            SetOptionalView(_completedView, false);
            return;
        }

        _button.interactable = _unlocked;

        _titleText.text = _levelConfig.DisplayName;
        _statusText.text = GetStatusText(completed);

        SetOptionalView(_lockedView, !_unlocked);
        SetOptionalView(_completedView, completed);
    }

    private string GetStatusText(bool completed)
    {
        if (!_unlocked)
            return "Закрыт";

        if (completed)
            return "Пройден";

        return "Доступен";
    }

    private void OnButtonClicked()
    {
        if (!_unlocked || _levelConfig == null)
            return;

        _onClicked?.Invoke(_levelConfig);
    }

    private static void SetOptionalView(GameObject view, bool active)
    {
        if (view == null)
            return;

        view.SetActive(active);
    }
}