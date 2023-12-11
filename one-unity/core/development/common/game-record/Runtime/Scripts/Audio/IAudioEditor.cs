using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IAudioEditor
{
    int GetTrackCount();

    Guid AddTrack(float[] sampleBuffer, int channels, bool isMainTrack = false, string label = default);

    Guid AddTrack(AudioClip audioClip, float volume = 1.0f, bool isMainTrack = false, string label = default);

    Guid AddTrack(AudioSource audioSource, float volume = 1.0f, bool isMainTrack = false, string label = default);

    void SetTrackVolume(Guid id, float volume);

    void PlayTrack(Guid id, long timestamp = default);

    void PauseTrack(Guid id);

    void StopTrack(Guid id);

    void PlayAllTracks();

    void PauseAllTracks();

    void UnPauseAllTracks();

    void StopAllTracks();

    UniTask SaveAsWav(string fileName);

    void Dispose();
}
