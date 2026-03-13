using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceStorageView : MonoBehaviour
{
    [SerializeField] private Text woodText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text meatText;

    private ResourceStorage subscribedStorage;

    private void OnEnable()
    {
        TrySubscribe();
        Refresh();
    }

    private void Update()
    {
        if (subscribedStorage == null && ResourceStorage.Instance != null)
        {
            TrySubscribe();
            Refresh();
        }
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        if (ResourceStorage.Instance == null)
            return;

        if (subscribedStorage == ResourceStorage.Instance)
            return;

        Unsubscribe();
        subscribedStorage = ResourceStorage.Instance;
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
        if (ResourceStorage.Instance == null)
            return;

        if (woodText != null)
            woodText.text = ResourceStorage.Instance.Wood.ToString();

        if (goldText != null)
            goldText.text = ResourceStorage.Instance.Gold.ToString();

        if (meatText != null)
            meatText.text = ResourceStorage.Instance.Meat.ToString();
    }
}
