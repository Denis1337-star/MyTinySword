using TMPro;
using UnityEngine;
using YG;

/// <summary>
/// Статичный TMP-текст с RU/EN и автообновлением при смене языка.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(TMP_Text))]
public sealed class LocalizedTmpText : MonoBehaviour
{
    [SerializeField] private string _ru;
    [SerializeField] private string _en;

    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();

        if (string.IsNullOrWhiteSpace(_ru))
            _ru = _text.text;
    }

    private void OnEnable()
    {
        YG2.onSwitchLang += Apply;
        Apply(YG2.lang);
    }

    private void OnDisable()
    {
        YG2.onSwitchLang -= Apply;
    }

    public void Setup(string ru, string en)
    {
        _ru = ru;
        _en = en;

        if (_text == null)
            _text = GetComponent<TMP_Text>();

        Apply(YG2.lang);
    }

    private void Apply(string lang)
    {
        if (_text == null)
            return;

        _text.text = Lang.Pick(_ru, _en);
    }
}
