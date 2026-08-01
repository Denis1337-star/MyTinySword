using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

/// <summary>
/// Кнопка x2 скорости: реклама → заряд 3 мин, повторный клик toggles 1x/2x.
/// Поставь объект сам на сцену уровня (например под InteractionPanelsCanvas),
/// настрой RectTransform и проставь ссылки в Inspector.
/// </summary>
public sealed class RewardedSpeedBoostButton : ValidatedMonoBehaviour
{
    [Header("Reward")]
    [SerializeField, Min(1f)] private float _rewardSeconds = GameSpeedBoostService.DefaultRewardSeconds;

    [Header("UI")]
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _speedStatusText;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _videoIcon;
    [SerializeField] private CanvasGroup _canvasGroup;

    private IAdvertisementService _advertisementService;
    private GameSpeedBoostService _speedBoostService;
    private bool _adInProgress;
    private int _lastDisplayedSeconds = -1;
    private bool _lastDisplayedBoostOn;
    private bool _lastDisplayedHasCharge;

    [Inject]
    private void Construct(
        IAdvertisementService advertisementService,
        GameSpeedBoostService speedBoostService)
    {
        _advertisementService = advertisementService;
        _speedBoostService = speedBoostService;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        RefreshView(force: true);
    }

    private void OnEnable()
    {
        if (_button != null)
            _button.onClick.AddListener(HandleClicked);

        if (_speedBoostService != null)
            _speedBoostService.StateChanged += HandleStateChanged;

        YG2.onSwitchLang += HandleSwitchLang;
        RefreshView(force: true);
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(HandleClicked);

        if (_speedBoostService != null)
            _speedBoostService.StateChanged -= HandleStateChanged;

        YG2.onSwitchLang -= HandleSwitchLang;
    }

    private void HandleSwitchLang(string lang)
    {
        RefreshView(force: true);
    }

    private void Update()
    {
        if (_speedBoostService == null)
            return;

        // Пока есть заряд — тикаем таймер. Если заряд только что кончился,
        // один раз ещё обновим UI (иначе кнопка залипает на «1 сек / 2x»).
        if (!_speedBoostService.HasCharge && !_lastDisplayedHasCharge)
            return;

        RefreshView(force: false);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;
        valid &= ValidationUtility.IsAssigned(this, _button, nameof(_button));
        valid &= ValidationUtility.IsAssigned(this, _timerText, nameof(_timerText));
        valid &= ValidationUtility.IsAssigned(this, _videoIcon, nameof(_videoIcon));
        return valid;
    }

    private void HandleClicked()
    {
        if (_adInProgress || _advertisementService.IsRewardedAdInProgress)
            return;

        if (_speedBoostService.HasCharge)
        {
            _speedBoostService.ToggleEnabled();
            return;
        }

        _adInProgress = true;
        RefreshView(force: true);

        _advertisementService.ShowRewardedAd(
            rewardId: RewardedAdIds.SpeedBoost2x,
            onRewarded: OnRewarded,
            onClosed: OnAdClosed,
            onError: OnAdError);
    }

    private void OnRewarded()
    {
        _speedBoostService.GrantReward(_rewardSeconds);
    }

    private void OnAdClosed()
    {
        _adInProgress = false;
        RefreshView(force: true);
    }

    private void OnAdError(string message)
    {
        _adInProgress = false;
        RefreshView(force: true);
    }

    private void HandleStateChanged()
    {
        RefreshView(force: true);
    }

    private void RefreshView(bool force)
    {
        if (_timerText == null || _button == null || _speedBoostService == null)
            return;

        int seconds = Mathf.CeilToInt(_speedBoostService.RemainingSeconds);
        bool hasCharge = _speedBoostService.HasCharge;
        bool boostOn = _speedBoostService.IsEnabled;

        if (!force
            && seconds == _lastDisplayedSeconds
            && boostOn == _lastDisplayedBoostOn
            && hasCharge == _lastDisplayedHasCharge)
            return;

        _lastDisplayedSeconds = seconds;
        _lastDisplayedBoostOn = boostOn;
        _lastDisplayedHasCharge = hasCharge;

        if (hasCharge)
        {
            _timerText.text = GameUiText.FormatMinutesSeconds(seconds);

            if (_speedStatusText != null)
                _speedStatusText.text = GameUiText.GameSpeedStatus(boostOn);
        }
        else
        {
            // До рекламы — награда (2x + длительность), не текущая 1x.
            int rewardSeconds = Mathf.CeilToInt(_rewardSeconds);
            _timerText.text = GameUiText.SpeedBoostRewardTitle;

            if (_speedStatusText != null)
                _speedStatusText.text = GameUiText.SpeedBoostRewardDuration(rewardSeconds);
        }

        // Иконка рекламы: только когда заряда нет.
        if (_videoIcon != null)
            _videoIcon.enabled = !hasCharge;

        _button.interactable = !_adInProgress;

        if (_canvasGroup != null)
            _canvasGroup.alpha = _adInProgress ? 0.7f : boostOn || !hasCharge ? 1f : 0.75f;

        if (_iconImage != null)
            _iconImage.color = boostOn || !hasCharge
                ? Color.white
                : new Color(0.75f, 0.75f, 0.75f, 1f);
    }
}
