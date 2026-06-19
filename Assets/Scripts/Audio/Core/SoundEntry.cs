using System;
using UnityEngine;

/// <summary>
/// Одна настройка звука
/// </summary>
[Serializable]
public sealed class SoundEntry
{
    [SerializeField] private SoundId _id;
    [SerializeField] private AudioClip[] _clips;
    [SerializeField, Range(0f, 1f)] private float _volume = 1f;
    [SerializeField, Range(0f, 0.3f)] private float _randomPitch = 0.05f;

    public SoundId Id => _id;
    public float Volume => _volume;

    public AudioClip GetClip()
    {
        if (_clips == null || _clips.Length == 0)
            return null;

        if (_clips.Length == 1)
            return _clips[0];

        int index = UnityEngine.Random.Range(0, _clips.Length);
        return _clips[index];
    }

    public float GetPitch()
    {
        if (_randomPitch <= 0f)
            return 1f;

        return UnityEngine.Random.Range(
            1f - _randomPitch,
            1f + _randomPitch);
    }
}