using TMPro;
using UnityEngine;
using YG;

public sealed class LocalizedTmpText : MonoBehaviour
{
    [SerializeField] private string _ru;
    [SerializeField] private string _en;
    [SerializeField]private TMP_Text _text;

    private void OnEnable()
    {
        YG2.onSwitchLang += ApplyLanguage;
        ApplyLanguage(YG2.lang);
    }

    private void OnDisable()
    {
        YG2.onSwitchLang -= ApplyLanguage;
    }

    private void ApplyLanguage(string lang)
    {
        if (_text == null)
            return;

        _text.text = lang == "ru" ? _ru : _en;
    }
}