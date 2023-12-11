using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using NatSuite.Recorders.Inputs;
using UnityEngine;

/*
 * Record video with pre-recorded audio.
 *
 * TODO: [TF3R-124] Support commit frames and audio samples.
 */
public class MP4VideoMuxer : IDisposable
{
    private readonly int videoWidth;
    private readonly int videoHeight;
    private readonly int frameRate;
    private MP4Recorder recorder;
    private AudioInput audioInput;
    private CameraInput cameraInput;
    private AudioSource currentAudioSource;
    private List<AudioSource> generatedAudioSources = new List<AudioSource>();
    private bool isRecording;
    private bool disposedValue = false;

    public MP4VideoMuxer(int videoWidth, int videoHeight, int frameRate)
    {
        this.videoWidth = videoWidth;
        this.videoHeight = videoHeight;
        this.frameRate = frameRate;
    }

    public void StartRecord(AudioListener audioListener, params Camera[] cameras)
    {
        if (isRecording)
        {
            throw new InvalidOperationException("Recording is already started.");
        }

        if (audioListener == null)
        {
            throw new ArgumentNullException(nameof(audioListener));
        }

        if (cameras == null)
        {
            throw new ArgumentNullException(nameof(cameras));
        }

        if (cameras.Length == 0)
        {
            throw new ArgumentException("At least one camera is required.", nameof(cameras));
        }

        isRecording = true;
        var clock = new RealtimeClock();
        var sampleRate = AudioSettings.outputSampleRate;
        var channelCount = (int)AudioSettings.GetConfiguration().speakerMode;
        recorder = new MP4Recorder(
            videoWidth,
            videoHeight,
            frameRate,
            channelCount: channelCount,
            sampleRate: sampleRate);
        cameraInput = new CameraInput(recorder, clock, cameras);
        audioInput = new AudioInput(recorder, clock, audioListener);
    }

    public void StartRecord(bool mute, AudioSource audioSource = default, params Camera[] cameras)
    {
        if (isRecording)
        {
            throw new InvalidOperationException("Recording is already started.");
        }

        if (cameras == null)
        {
            throw new ArgumentNullException(nameof(cameras));
        }

        if (cameras.Length == 0)
        {
            throw new ArgumentException("At least one camera is required.", nameof(cameras));
        }

        isRecording = true;
        var clock = new RealtimeClock();
        var sampleRate = AudioSettings.outputSampleRate;
        recorder = new MP4Recorder(
            videoWidth,
            videoHeight,
            frameRate,
            sampleRate: sampleRate,
            channelCount: (audioSource == default) ? 0 : audioSource.clip.channels);
        cameraInput = new CameraInput(recorder, clock, cameras);

        if (audioSource != default)
        {
            currentAudioSource = audioSource;
            audioInput = new AudioInput(recorder, clock, audioSource, mute);
            currentAudioSource.Play();
        }
    }

    public void StartRecord(AudioClip audioClip, bool mute, params Camera[] cameras)
    {
        currentAudioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
        currentAudioSource.clip = audioClip;
        currentAudioSource.spatialBlend = 0;
        currentAudioSource.volume = 1;
        generatedAudioSources.Add(currentAudioSource);
        StartRecord(mute, currentAudioSource, cameras);
    }

    public void StopRecord()
    {
        if (recorder == null)
        {
            throw new InvalidOperationException("Recording is not started.");
        }

        if (currentAudioSource && currentAudioSource.isPlaying)
        {
            currentAudioSource.Stop();
        }

        audioInput?.Dispose();
        cameraInput.Dispose();
        cameraInput = null;
        isRecording = false;
    }

    public UniTask<string> Export()
    {
        if (recorder == null)
        {
            throw new InvalidOperationException("Recording is not started.");
        }

        return recorder.FinishWriting().AsUniTask();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                generatedAudioSources.ForEach(audioSource => UnityEngine.Object.Destroy(audioSource.gameObject));
                audioInput?.Dispose();
                cameraInput?.Dispose();
                cameraInput = null;
                recorder = null;
            }

            disposedValue = true;
        }
    }
}
