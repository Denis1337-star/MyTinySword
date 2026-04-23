
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Один элемент панели выбранной армии:
/// показывает иконку класса юнита и количество.
/// </summary>
public class ArmySelectionItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text countText;

    /// <summary>
    /// Заполняет UI-элемент данными.
    /// </summary>
    public void Bind(Sprite icon, int count)
    {
        if (iconImage != null)
            iconImage.sprite = icon;

        if (countText != null)
            countText.text = $"x{count}";
    }
}
