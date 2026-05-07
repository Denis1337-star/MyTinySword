using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI элемент одного типа боевых юнитов в панели выбранной армии
/// </summary>
public sealed class ArmySelectionItem : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _countText;

    public void Bind(Sprite icon, int count)
    {
        if (_iconImage != null)
            _iconImage.sprite = icon;

        if (_countText != null)
            _countText.text = count.ToString();
    }
}