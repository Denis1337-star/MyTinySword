using UnityEngine;
using YG;

/// <summary>
/// ?????? ?????????? ? ???????? ????? ???????? ????? PluginYG2 Storage.
/// </summary>
public sealed class YandexAudioSaveService
{
    public void LoadTo(GameAudioService audioService)
    {
        if (!YG2.saves.audioSettingsInitialized)
        {
            SaveFrom(audioService);
            return;
        }

        audioService.SetMusicVolume(YG2.saves.musicVolume);
        audioService.SetSfxVolume(YG2.saves.sfxVolume);
        audioService.SetMusicMuted(YG2.saves.musicMuted);
        audioService.SetSfxMuted(YG2.saves.sfxMuted);
    }

    public void SaveFrom(GameAudioService audioService)
    {
        YG2.saves.audioSettingsInitialized = true;
        YG2.saves.musicVolume = audioService.MusicVolume;
        YG2.saves.sfxVolume = audioService.SfxVolume;
        YG2.saves.musicMuted = audioService.IsMusicMuted;
        YG2.saves.sfxMuted = audioService.IsSfxMuted;

        YandexSaveUtility.SaveProgress();
    }
}
