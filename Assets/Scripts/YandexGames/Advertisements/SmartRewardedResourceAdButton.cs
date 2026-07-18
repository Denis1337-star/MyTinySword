using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Умная rewarded-кнопка ресурса.
/// Показывается только когда хотя бы один ресурс ниже порога.
/// Выбирает самый дефицитный ресурс и выдаёт награду после просмотра.
///
/// Важно:
/// Кнопка не отключается во время рекламы.
/// Скрывается через _viewRoot.
/// </summary>
public sealed class SmartRewardedResourceAdButton : ValidatedMonoBehaviour
{
    [Header("Rules")]
    [SerializeField, Min(0)] private int _lowResourceThreshold = 30;
    [SerializeField, Min(1)] private int _rewardAmount = RewardedAdIds.DefaultRewardAmount;

    [Header("Root")]
    [SerializeField] private GameObject _viewRoot;

    [Header("UI")]
    [SerializeField] private Button _button;
    [SerializeField] private Image _videoIcon;
    [SerializeField] private Image _resourceIcon;
    [SerializeField] private TMP_Text _amountText;

    [Header("Resource Icons")]
    [SerializeField] private Sprite _woodIcon;
    [SerializeField] private Sprite _meatIcon;
    [SerializeField] private Sprite _goldIcon;

    private IAdvertisementService _advertisementService;
    private ResourceStorage _resourceStorage;

    private ResourceType _currentRewardType = ResourceType.None;
    private bool _adInProgress;

    [Inject]
    private void Construct(
        IAdvertisementService advertisementService,
        ResourceStorage resourceStorage)
    {
        _advertisementService = advertisementService;
        _resourceStorage = resourceStorage;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        RefreshView();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ShowRewardedAd);

        if (_resourceStorage != null)
            _resourceStorage.ResourcesChanged += RefreshView;

        RefreshView();
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(ShowRewardedAd);

        if (_resourceStorage != null)
            _resourceStorage.ResourcesChanged -= RefreshView;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _viewRoot, nameof(_viewRoot));
        valid &= ValidationUtility.IsAssigned(this, _button, nameof(_button));
        valid &= ValidationUtility.IsAssigned(this, _videoIcon, nameof(_videoIcon));
        valid &= ValidationUtility.IsAssigned(this, _resourceIcon, nameof(_resourceIcon));
        valid &= ValidationUtility.IsAssigned(this, _amountText, nameof(_amountText));

        valid &= ValidationUtility.IsAssigned(this, _woodIcon, nameof(_woodIcon));
        valid &= ValidationUtility.IsAssigned(this, _meatIcon, nameof(_meatIcon));
        valid &= ValidationUtility.IsAssigned(this, _goldIcon, nameof(_goldIcon));

        return valid;
    }

    private void ShowRewardedAd()
    {
        if (_adInProgress)
            return;

        if (_advertisementService.IsRewardedAdInProgress)
            return;

        if (!TryFindLowestLowResource(out ResourceType rewardType))
        {
            RefreshView();
            return;
        }

        _currentRewardType = rewardType;
        _adInProgress = true;

        RefreshView();

        _advertisementService.ShowRewardedAd(
            rewardId: RewardedAdIds.ForResource(_currentRewardType, _rewardAmount),
            onRewarded: GiveReward,
            onClosed: OnAdClosed,
            onError: OnAdError);
    }

    private void GiveReward()
    {
        if (_currentRewardType == ResourceType.None)
            return;

        _resourceStorage.AddResource(_currentRewardType, _rewardAmount);
    }

    private void OnAdClosed()
    {
        _adInProgress = false;
        _currentRewardType = ResourceType.None;
        RefreshView();
    }

    private void OnAdError(string message)
    {
        _adInProgress = false;
        _currentRewardType = ResourceType.None;
        RefreshView();
    }

    private void RefreshView()
    {
        if (_viewRoot == null || _button == null || _resourceIcon == null || _amountText == null)
            return;

        if (_adInProgress)
        {
            _viewRoot.SetActive(true);
            _button.interactable = false;
            _amountText.text = "...";
            return;
        }

        bool hasReward = TryFindLowestLowResource(out ResourceType rewardType);

        _currentRewardType = rewardType;
        _viewRoot.SetActive(hasReward);

        if (!hasReward)
            return;

        _button.interactable = true;
        _amountText.text = $"+{_rewardAmount}";
        _resourceIcon.sprite = GetResourceIcon(rewardType);
        _resourceIcon.enabled = _resourceIcon.sprite != null;

        if (_videoIcon != null)
            _videoIcon.enabled = true;
    }

    private bool TryFindLowestLowResource(out ResourceType resourceType)
    {
        resourceType = ResourceType.None;

        int wood = _resourceStorage.GetAmount(ResourceType.Wood);
        int meat = _resourceStorage.GetAmount(ResourceType.Meat);
        int gold = _resourceStorage.GetAmount(ResourceType.Gold);

        bool woodLow = wood < _lowResourceThreshold;
        bool meatLow = meat < _lowResourceThreshold;
        bool goldLow = gold < _lowResourceThreshold;

        if (!woodLow && !meatLow && !goldLow)
            return false;

        int bestAmount = int.MaxValue;

        if (woodLow && wood < bestAmount)
        {
            bestAmount = wood;
            resourceType = ResourceType.Wood;
        }

        if (meatLow && meat < bestAmount)
        {
            bestAmount = meat;
            resourceType = ResourceType.Meat;
        }

        if (goldLow && gold < bestAmount)
            resourceType = ResourceType.Gold;

        return resourceType != ResourceType.None;
    }

    private Sprite GetResourceIcon(ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Wood => _woodIcon,
            ResourceType.Meat => _meatIcon,
            ResourceType.Gold => _goldIcon,
            _ => null
        };
    }
}
