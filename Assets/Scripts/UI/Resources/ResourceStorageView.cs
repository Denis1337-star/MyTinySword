using TMPro;
using UnityEngine;
using Zenject;

/// <summary>
/// UI отображение ресурсов 
/// </summary>
public sealed class ResourceStorageView : ValidatedMonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text _woodText;
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private TMP_Text _meatText;

    private ResourceStorage _resourceStorage;

    [Inject]
    private void Construct(ResourceStorage resourceStorage)
    {
        _resourceStorage = resourceStorage;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _woodText, nameof(_woodText));
        valid &= ValidationUtility.IsAssigned(this, _goldText, nameof(_goldText));
        valid &= ValidationUtility.IsAssigned(this, _meatText, nameof(_meatText));

        return valid;
    }

    private void Start()
    {
        _resourceStorage.ResourcesChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        _resourceStorage.ResourcesChanged -= Refresh;
    }

    private void Refresh()
    {
        _woodText.text = _resourceStorage.Wood.ToString();
        _goldText.text = _resourceStorage.Gold.ToString();
        _meatText.text = _resourceStorage.Meat.ToString();
    }
}