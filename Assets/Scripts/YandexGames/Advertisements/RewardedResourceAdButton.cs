using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Универсальная rewarded кнопка для выдачи ресурсов
/// </summary>
public sealed class RewardedResourceAdButton : ValidatedMonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private string _rewardId = "wood_40";
    [SerializeField] private ResourceType _resourceType = ResourceType.Wood;
    [SerializeField, Min(1)] private int _amount = 40;

    [Header("UI")]
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _label;

    private IAdvertisementService _advertisementService;
    private ResourceStorage _resourceStorage;

    [Inject]
    private void Construct(
        IAdvertisementService advertisementService,
        ResourceStorage resourceStorage)
    {
        _advertisementService = advertisementService;
        _resourceStorage = resourceStorage;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ShowRewardedAd);
        RefreshView(false);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ShowRewardedAd);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _button, nameof(_button));
        valid &= ValidationUtility.IsAssigned(this, _label, nameof(_label));

        if (string.IsNullOrWhiteSpace(_rewardId))
        {
            Debug.LogError($"{name}: Reward Id не задан.", this);
            valid = false;
        }

        if (_amount <= 0)
        {
            Debug.LogError($"{name}: количество награды должно быть больше 0.", this);
            valid = false;
        }

        return valid;
    }

    private void ShowRewardedAd()
    {
        if (_advertisementService.IsRewardedAdInProgress)
            return;

        RefreshView(true);

        _advertisementService.ShowRewardedAd(
            rewardId: _rewardId,
            onRewarded: GiveReward,
            onClosed: OnAdClosed,
            onError: OnAdError);
    }

    private void GiveReward()
    {
        _resourceStorage.AddResource(_resourceType, _amount);

        Debug.Log(
            $"[RewardedResourceAdButton] Игрок получил +{_amount} {GetResourceName(_resourceType)} за рекламу.",
            this);
    }

    private void OnAdClosed()
    {
        RefreshView(false);
    }

    private void OnAdError(string message)
    {
        Debug.LogWarning(
            $"[RewardedResourceAdButton] Реклама не показалась: {message}",
            this);

        RefreshView(false);
    }

    private void RefreshView(bool adInProgress)
    {
        _button.interactable = !adInProgress;

        if (_label == null)
            return;

        _label.text = adInProgress
            ? "Реклама..."
            : $"+{_amount} {GetResourceName(_resourceType)}";
    }

    private static string GetResourceName(ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Wood => "дерева",
            ResourceType.Meat => "еды",
            ResourceType.Gold => "золота",
            _ => "ресурса"
        };
    }
}