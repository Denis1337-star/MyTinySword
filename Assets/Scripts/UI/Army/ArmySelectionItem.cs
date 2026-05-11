using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI элемент одного типа боевых юнитов в панели выбранной армии
/// </summary>
public sealed class ArmySelectionItem : ValidatedMonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _countText;

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _iconImage, nameof(_iconImage));
        valid &= ValidationUtility.IsAssigned(this, _countText, nameof(_countText));

        return valid;
    }

    public void Bind(Sprite icon, int count)
    {
        _iconImage.sprite = icon;
        _countText.text = count.ToString();
    }
}