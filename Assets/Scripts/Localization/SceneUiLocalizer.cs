using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

/// <summary>
/// Локализует статичные TMP-подписи на сцене: кнопки и известные заголовки.
/// Динамические поля панелей не трогает.
/// </summary>
public sealed class SceneUiLocalizer : MonoBehaviour
{
    private TMP_Text[] _texts;
    private string[] _russianTexts;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        LocalizeScene(SceneManager.GetActiveScene());
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LocalizeScene(scene);
    }

    private static void LocalizeScene(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return;

        GameObject host = new($"SceneUiLocalizer_{scene.name}");
        SceneManager.MoveGameObjectToScene(host, scene);
        host.AddComponent<SceneUiLocalizer>();
    }

    private void Awake()
    {
        CacheTexts();
        ApplyLanguage(YG2.lang);
        YG2.onSwitchLang += ApplyLanguage;
    }

    private void OnDestroy()
    {
        YG2.onSwitchLang -= ApplyLanguage;
    }

    private void CacheTexts()
    {
        TMP_Text[] allTexts = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var texts = new System.Collections.Generic.List<TMP_Text>();
        var russianTexts = new System.Collections.Generic.List<string>();

        for (int i = 0; i < allTexts.Length; i++)
        {
            TMP_Text text = allTexts[i];

            if (text == null || text.GetComponent<LocalizedTmpText>() != null)
                continue;

            string russian = text.text;

            if (!ShouldLocalize(text, russian))
                continue;

            texts.Add(text);
            russianTexts.Add(russian);
        }

        _texts = texts.ToArray();
        _russianTexts = russianTexts.ToArray();
    }

    private static bool ShouldLocalize(TMP_Text text, string russian)
    {
        if (string.IsNullOrWhiteSpace(russian))
            return false;

        if (text.GetComponentInParent<Button>() != null)
            return true;

        return GameUiText.TryGetStaticSceneEnglish(russian.Trim(), out _);
    }

    private void ApplyLanguage(string lang)
    {
        if (_texts == null)
            return;

        for (int i = 0; i < _texts.Length; i++)
        {
            TMP_Text text = _texts[i];

            if (text == null)
                continue;

            string russian = _russianTexts[i];

            if (string.IsNullOrWhiteSpace(russian))
                continue;

            if (lang == Lang.English && GameUiText.TryGetStaticSceneEnglish(russian.Trim(), out string english))
                text.text = english;
            else
                text.text = russian;
        }
    }
}
