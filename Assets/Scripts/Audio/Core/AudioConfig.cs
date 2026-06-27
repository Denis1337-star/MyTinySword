using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Главный конфиг звука
/// </summary>
[CreateAssetMenu(
    fileName = "AudioConfig",
    menuName = "MyTinySword/Audio/Audio Config")]
public sealed class AudioConfig : ScriptableObject
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    [Header("Mixer Exposed Parameters")]
    [SerializeField] private string _musicVolumeParameter = "MusicVolume";
    [SerializeField] private string _sfxVolumeParameter = "SfxVolume";

    [Header("Default Volumes")]
    [SerializeField, Range(0f, 1f)] private float _defaultMusicVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float _defaultSfxVolume = 0.8f;

    [Header("Music")]
    [SerializeField] private SceneMusicEntry[] _sceneMusic;

    [Header("Sounds")]
    [SerializeField] private SoundEntry[] _sounds;

    [Header("World SFX Pool")]
    [SerializeField, Min(1)] private int _worldSfxPoolSize = 16;

    [Header("World SFX Distance")]
    [SerializeField, Min(0.1f)] private float _worldMinDistance = 2f;
    [SerializeField, Min(0.1f)] private float _worldMaxDistance = 14f;

    public AudioMixer AudioMixer => _audioMixer;
    public AudioMixerGroup MusicMixerGroup => _musicMixerGroup;
    public AudioMixerGroup SfxMixerGroup => _sfxMixerGroup;

    public string MusicVolumeParameter => _musicVolumeParameter;
    public string SfxVolumeParameter => _sfxVolumeParameter;

    public float DefaultMusicVolume => _defaultMusicVolume;
    public float DefaultSfxVolume => _defaultSfxVolume;

    public int WorldSfxPoolSize => _worldSfxPoolSize;
    public float WorldMinDistance => _worldMinDistance;
    public float WorldMaxDistance => _worldMaxDistance;

    public bool IsValid()
    {
        bool valid = true;

        if (_audioMixer == null)
        {
            Debug.LogError($"{name}: Audio Mixer не назначен.", this);
            valid = false;
        }

        if (_musicMixerGroup == null)
        {
            Debug.LogError($"{name}: Music Mixer Group не назначен.", this);
            valid = false;
        }

        if (_sfxMixerGroup == null)
        {
            Debug.LogError($"{name}: Sfx Mixer Group не назначен.", this);
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(_musicVolumeParameter))
        {
            Debug.LogError($"{name}: Music Volume Parameter не задан.", this);
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(_sfxVolumeParameter))
        {
            Debug.LogError($"{name}: Sfx Volume Parameter не задан.", this);
            valid = false;
        }

        if (_worldSfxPoolSize < 1)
        {
            Debug.LogError($"{name}: World Sfx Pool Size должен быть больше 0.", this);
            valid = false;
        }

        if (_worldMinDistance <= 0f || _worldMaxDistance <= 0f)
        {
            Debug.LogError($"{name}: World SFX distance настроен некорректно.", this);
            valid = false;
        }

        return valid;
    }

    public AudioClip GetMusicForScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return null;

        if (_sceneMusic == null)
            return null;

        for (int i = 0; i < _sceneMusic.Length; i++)
        {
            SceneMusicEntry entry = _sceneMusic[i];

            if (entry == null)
                continue;

            if (entry.SceneName == sceneName)
                return entry.MusicClip;
        }

        return null;
    }

    public SoundEntry GetSound(SoundId id)
    {
        if (id == SoundId.None)
            return null;

        if (_sounds == null)
            return null;

        for (int i = 0; i < _sounds.Length; i++)
        {
            SoundEntry entry = _sounds[i];

            if (entry == null)
                continue;

            if (entry.Id == id)
                return entry;
        }

        return null;
    }
}