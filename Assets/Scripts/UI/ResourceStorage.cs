using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceStorage : MonoBehaviour
{
    public static ResourceStorage Instance { get; private set; }

    [SerializeField] private int wood;
    [SerializeField] private int gold;
    [SerializeField] private Text woodText;
    [SerializeField] private Text goldText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        UpdateUI();
    }

    public void AddWood(int amount)
    {
        wood += amount;
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (woodText != null)
            woodText.text = wood.ToString();
        if (goldText != null)
            goldText.text = gold.ToString();
    }
}
