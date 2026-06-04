using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Тестовая rewarded-кнопка: посмотреть рекламу и получить дерево.
/// Позже заменим на универсальную систему RewardDefinition.
/// </summary>
public sealed class RewardedWoodButton : ValidatedMonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _label;
    [SerializeField, Min(1)] private int _woodAmount = 40;

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

        return valid;
    }

    private void ShowRewardedAd()
    {
        if (_advertisementService.IsRewardedAdInProgress)
            return;

        RefreshView(true);

        _advertisementService.ShowRewardedAd(
            onRewarded: GiveReward,
            onClosed: OnAdClosed,
            onError: OnAdError);
    }

    private void GiveReward()
    {
        _resourceStorage.AddResource(ResourceType.Wood, _woodAmount);
        Debug.Log($"[RewardedWoodButton] Игрок получил +{_woodAmount} дерева за рекламу.", this);
    }

    private void OnAdClosed()
    {
        RefreshView(false);
    }

    private void OnAdError(string message)
    {
        Debug.LogWarning($"[RewardedWoodButton] Реклама не показалась: {message}", this);
        RefreshView(false);
    }

    private void RefreshView(bool adInProgress)
    {
        _button.interactable = !adInProgress;

        if (_label != null)
            _label.text = adInProgress
                ? "Реклама..."
                : $"+{_woodAmount} дерева за рекламу";
    }
}