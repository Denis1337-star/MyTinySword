using YG;

namespace YG
{
    /// <summary>
    /// Поля сохранения аудио настроек
    /// PluginYG2 хранит эти данные внутри YG2 saves
    /// </summary>
    public partial class SavesYG
    {
        public bool audioSettingsInitialized;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        public bool musicMuted;
        public bool sfxMuted;
    }
}