using UnityEngine;
using UnityEngine.UI;

public class ResourceStorage : MonoBehaviour
{
    public static ResourceStorage Instance { get; private set; }

    [SerializeField] private int wood;
    [SerializeField] private int gold;
    [SerializeField] private int meat;
    [SerializeField] private Text woodText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text meatText;

    private void Awake()
    {
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
    public void AddMeat(int amount)
    {
        meat += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (woodText != null)
            woodText.text = wood.ToString();
        if (goldText != null)
            goldText.text = gold.ToString();
        if (meatText != null)
            meatText.text = meat.ToString();
    }
}
