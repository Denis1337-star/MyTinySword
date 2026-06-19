using System;
using UnityEngine;

/// <summary>
/// Музыка для конкретной сцены
/// </summary>
[Serializable]
public sealed class SceneMusicEntry
{
    [SerializeField] private string _sceneName;
    [SerializeField] private AudioClip _musicClip;

    public string SceneName => _sceneName;
    public AudioClip MusicClip => _musicClip;
}