using UnityEngine;
using UnityEngine.UI;

public class ResourceStorageView : MonoBehaviour
{
    [SerializeField] private Text woodText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text meatText;

    private ResourceStorage subscribedStorage;

    private void Awake()
    {
        TrySubscribe();
        Refresh();
    }

    private void OnEnable()
    {
        TrySubscribe();
        Refresh();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        ResourceStorage storage = ResourceStorage.Instance;
        if (storage == null)
            return;

        if (subscribedStorage == storage)
            return;

        Unsubscribe();

        subscribedStorage = storage;
        subscribedStorage.OnResourcesChanged += Refresh;
    }

    private void Unsubscribe()
    {
        if (subscribedStorage == null)
            return;

        subscribedStorage.OnResourcesChanged -= Refresh;
        subscribedStorage = null;
    }

    private void Refresh()
    {
        ResourceStorage storage = ResourceStorage.Instance;
        if (storage == null)
            return;

        if (woodText != null)
            woodText.text = storage.Wood.ToString();

        if (goldText != null)
            goldText.text = storage.Gold.ToString();

        if (meatText != null)
            meatText.text = storage.Meat.ToString();
    }
}
