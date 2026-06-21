using System;
using YG;

/// <summary>
/// Источник времени для дерева развития
/// Сейчас использует UTC-время устройства
/// но не даёт времени идти назад относительно сохранения
/// </summary>
public sealed class TechTreeTimeService
{
    public long GetCurrentUnixTime()
    {
        long currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (YG2.saves.techTree == null)
            return currentUnixTime;

        if (currentUnixTime < YG2.saves.techTree.LastKnownUnixTime)
            return YG2.saves.techTree.LastKnownUnixTime;

        return currentUnixTime;
    }

    public void UpdateLastKnownTime()
    {
        if (YG2.saves.techTree == null)
            return;

        long currentUnixTime = GetCurrentUnixTime();

        if (currentUnixTime <= YG2.saves.techTree.LastKnownUnixTime)
            return;

        YG2.saves.techTree.LastKnownUnixTime = currentUnixTime;
        YandexSaveUtility.SaveProgress();
    }
}