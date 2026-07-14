using TMPro;
using UnityEngine;
using YG;

public sealed class LanguageToggleButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;


    private void OnEnable()
    {
        YG2.onSwitchLang += UpdateLabel;
        UpdateLabel(YG2.lang);
    }

    private void OnDisable()
    {
        YG2.onSwitchLang -= UpdateLabel;
    }

    public void ToggleLanguage()
    {
        string next = YG2.lang == "ru" ? "en" : "ru";
        YG2.SwitchLanguage(next);
    }

    private void UpdateLabel(string lang)
    {
        if (_label == null)
            return;

        _label.text = lang == "ru" ? "Русский" : "English";
    }
}