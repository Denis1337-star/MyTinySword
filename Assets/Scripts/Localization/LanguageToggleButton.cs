using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

/// <summary>
/// Кнопка переключения языка RU/EN через PluginYG2.
/// </summary>
[RequireComponent(typeof(Button))]
public sealed class LanguageToggleButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        if (_label == null)
            _label = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ToggleLanguage);
        YG2.onSwitchLang += RefreshLabel;
        RefreshLabel(YG2.lang);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ToggleLanguage);
        YG2.onSwitchLang -= RefreshLabel;
    }

    private void ToggleLanguage()
    {
        string nextLanguage = YG2.lang == Lang.English ? Lang.Russian : Lang.English;
        YG2.SwitchLanguage(nextLanguage);
    }

    private void RefreshLabel(string lang)
    {
        if (_label == null)
            return;

        _label.text = lang == Lang.English ? "Русский" : "English";
    }
}
