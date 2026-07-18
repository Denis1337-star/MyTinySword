using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

/// <summary>
/// Локализует стартовую InfoPanel на Level_2..Level_5:
/// текст подсказки + кнопка «Понятно».
/// </summary>
public sealed class LevelInfoPanelLocalizer : MonoBehaviour
{
    private TMP_Text _messageText;
    private string _messageRu;
    private string _messageEn;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        TryAttach(SceneManager.GetActiveScene());
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryAttach(scene);
    }

    private static void TryAttach(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return;

        if (!GameUiText.TryGetLevelInfoMessage(scene.name, out _, out _))
            return;

        GameObject infoPanel = FindInfoPanel(scene);

        if (infoPanel == null)
            return;

        if (infoPanel.GetComponent<LevelInfoPanelLocalizer>() != null)
            return;

        infoPanel.AddComponent<LevelInfoPanelLocalizer>();
    }

    private static GameObject FindInfoPanel(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();

        for (int i = 0; i < roots.Length; i++)
        {
            Transform found = FindChildByName(roots[i].transform, "InfoPanel");

            if (found != null)
                return found.gameObject;
        }

        return null;
    }

    private static Transform FindChildByName(Transform root, string objectName)
    {
        if (root.name == objectName)
            return root;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindChildByName(root.GetChild(i), objectName);

            if (found != null)
                return found;
        }

        return null;
    }

    private void Awake()
    {
        CacheTexts();
        Apply(YG2.lang);
    }

    private void OnEnable()
    {
        Subscribe();
        Apply(YG2.lang);
    }

    private void Start()
    {
        // PluginYG2 в Editor сбрасывает onSwitchLang при Init.
        Subscribe();
        Apply(YG2.lang);
    }

    private void OnDisable()
    {
        YG2.onSwitchLang -= Apply;
    }

    private void Subscribe()
    {
        YG2.onSwitchLang -= Apply;
        YG2.onSwitchLang += Apply;
    }

    private void CacheTexts()
    {
        string sceneName = gameObject.scene.name;

        if (!GameUiText.TryGetLevelInfoMessage(sceneName, out _messageRu, out _messageEn))
            return;

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        TMP_Text bestMessage = null;
        int bestLength = -1;

        for (int i = 0; i < texts.Length; i++)
        {
            TMP_Text text = texts[i];

            if (text == null)
                continue;

            if (text.GetComponentInParent<Button>() != null)
            {
                EnsureButtonLocalized(text);
                continue;
            }

            int length = string.IsNullOrEmpty(text.text) ? 0 : text.text.Length;

            if (length > bestLength)
            {
                bestLength = length;
                bestMessage = text;
            }
        }

        _messageText = bestMessage;
    }

    private static void EnsureButtonLocalized(TMP_Text text)
    {
        if (text.GetComponent<LocalizedTmpText>() != null)
            return;

        LocalizedTmpText localized = text.gameObject.AddComponent<LocalizedTmpText>();
        localized.Setup(GameUiText.OkRu, GameUiText.OkEn);
    }

    private void Apply(string lang)
    {
        if (_messageText == null)
            return;

        _messageText.text = Lang.Pick(_messageRu, _messageEn);
    }
}
