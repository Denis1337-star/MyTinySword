using TMPro;
using UnityEngine;
using YG;

/// <summary>
/// Статичный TMP-текст с RU/EN и автообновлением при смене языка PluginYG2.
/// Для динамических полей панелей используй GameUiText + Refresh().
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(TMP_Text))]
public sealed class LocalizedTmpText : MonoBehaviour
{
    [SerializeField] private string _ru;
    [SerializeField] private string _en;
    [SerializeField] private TMP_Text _text;

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<TMP_Text>();

        if (string.IsNullOrWhiteSpace(_ru) && _text != null)
            _ru = _text.text;
    }

    private void OnEnable()
    {
        Subscribe();
        Apply(YG2.lang);
    }

    private void Start()
    {
        // PluginYG2 в Editor сбрасывает onSwitchLang при Init — переподписываемся после него.
        Subscribe();
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

    private void Subscribe()
    {
        YG2.onSwitchLang -= Apply;
        YG2.onSwitchLang += Apply;
    }

    private void Apply(string lang)
    {
        if (_text == null)
            return;

        _text.text = Lang.Pick(_ru, _en);
    }
}
