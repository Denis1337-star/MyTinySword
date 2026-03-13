using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceStorageView : MonoBehaviour
{
    [SerializeField] private Text woodText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text meatText;

    private void OnEnable()
    {
        if (ResourceStorage.Instance != null)
            ResourceStorage.Instance.OnResourcesChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (ResourceStorage.Instance != null)
            ResourceStorage.Instance.OnResourcesChanged -= Refresh;
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
