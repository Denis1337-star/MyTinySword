using TMPro;
using UnityEngine;
using YG;

/// <summary>
/// Кнопка переключения языка RU ↔ EN через PluginYG2.
/// Вызови ToggleLanguage() из Button OnClick в инспекторе.
/// </summary>
public sealed class LanguageToggleButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;

    private void OnEnable()
    {
        Subscribe();
        RefreshLabel(YG2.lang);
    }

    private void Start()
    {
        // PluginYG2 в Editor сбрасывает onSwitchLang при Init — переподписываемся после него.
        Subscribe();
        RefreshLabel(YG2.lang);
    }

    private void OnDisable()
    {
        YG2.onSwitchLang -= RefreshLabel;
    }

    public void ToggleLanguage()
    {
        string nextLanguage = YG2.lang == Lang.English ? Lang.Russian : Lang.English;
        YG2.SwitchLanguage(nextLanguage);
    }

    private void Subscribe()
    {
        YG2.onSwitchLang -= RefreshLabel;
        YG2.onSwitchLang += RefreshLabel;
    }

    private void RefreshLabel(string lang)
    {
        if (_label == null)
            return;

        // Показываем язык, на который можно переключиться.
        _label.text = lang == Lang.English ? "Русский" : "English";
    }
}
