using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

/// <summary>
/// Авто-локализация статичных TMP на сцене (кнопки и известные подписи).
/// Не трогает объекты с LocalizedTmpText и динамические поля панелей.
/// На MainMenu обычно уже стоят LocalizedTmpText — Localizer там пропускает их.
/// </summary>
public sealed class SceneUiLocalizer : MonoBehaviour
{
    private TMP_Text[] _texts;
    private string[] _russianTexts;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
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

        // Не плодим несколько локализаторов на одну сцену.
        SceneUiLocalizer[] existing = FindObjectsByType<SceneUiLocalizer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < existing.Length; i++)
        {
            if (existing[i] != null && existing[i].gameObject.scene == scene)
                return;
        }

        GameObject host = new($"SceneUiLocalizer_{scene.name}");
        SceneManager.MoveGameObjectToScene(host, scene);
        host.AddComponent<SceneUiLocalizer>();
    }

    private void Awake()
    {
        CacheTexts();
        Subscribe();
        ApplyLanguage(YG2.lang);
    }

    private void Start()
    {
        Subscribe();
        ApplyLanguage(YG2.lang);
    }

    private void OnDestroy()
    {
        YG2.onSwitchLang -= ApplyLanguage;
    }

    private void Subscribe()
    {
        YG2.onSwitchLang -= ApplyLanguage;
        YG2.onSwitchLang += ApplyLanguage;
    }

    private void CacheTexts()
    {
        TMP_Text[] allTexts = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var texts = new List<TMP_Text>();
        var russianTexts = new List<string>();

        for (int i = 0; i < allTexts.Length; i++)
        {
            TMP_Text text = allTexts[i];

            if (text == null || text.GetComponent<LocalizedTmpText>() != null)
                continue;

            // Не перехватываем кнопку языка — ею управляет LanguageToggleButton.
            if (text.GetComponentInParent<LanguageToggleButton>() != null)
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
            return GameUiText.TryGetStaticSceneEnglish(russian.Trim(), out _);

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
