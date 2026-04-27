using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

/// <summary>
/// UI-отображение ресурсов игрока
/// </summary>
public class ResourceStorageView : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text _woodText;
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private TMP_Text _meatText;

    private readonly CompositeDisposable _disposables = new();

    private ResourceStorage _resourceStorage;

    [Inject]
    private void Construct(ResourceStorage resourceStorage)
    {
        _resourceStorage = resourceStorage;
    }

    private void Start()
    {
        if (_resourceStorage == null)
        {
            Debug.LogError($"{name}: ResourceStorage не внедрён через Zenject.", this);
            return;
        }

        _resourceStorage.ResourcesChanged
            .Subscribe(_ => Refresh())
            .AddTo(_disposables);

        Refresh();
    }

    private void Refresh()
    {
        if (_resourceStorage == null)
            return;

        if (_woodText != null)
            _woodText.text = _resourceStorage.Wood.ToString();

        if (_goldText != null)
            _goldText.text = _resourceStorage.Gold.ToString();

        if (_meatText != null)
            _meatText.text = _resourceStorage.Meat.ToString();
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
