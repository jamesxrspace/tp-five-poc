using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using UnityEngine;

namespace TPFive.Game.Record
{
    public class DefaultAudioEditor : IAudioEditor, IDisposable
    {
        private const int DefaultFrequency = 44100;

        private List<Track> tracks = new List<Track>();
        private GameObject audioEditorGo;
        private bool disposedValue = false;

        public DefaultAudioEditor()
        {
        }

        public int GetTrackCount()
        {
            return tracks.Count;
        }

        public Guid AddTrack(float[] sampleBuffer, int channels, bool isMainTrack = false, string label = default)
        {
            if (sampleBuffer == null || sampleBuffer.Length == 0)
            {
                throw new ArgumentException("Sample buffer is empty.", nameof(sampleBuffer));
            }

            if (channels < 0 || channels > 2)
            {
                throw new ArgumentException($"{channels} channels is not supported", nameof(channels));
            }

            var audioClip = AudioClip.Create(label, sampleBuffer.Length, channels, DefaultFrequency, false);
            audioClip.SetData(sampleBuffer, 0);
            return AddTrack(audioClip, isMainTrack: isMainTrack, label: label);
        }

        public Guid AddTrack(AudioClip audioClip, float volume = 1.0f, bool isMainTrack = false, string label = default)
        {
            if (audioClip == null)
            {
                throw new ArgumentException("Audio clip is empty.", nameof(audioClip));
            }

            var audioSource = CreateAudioSource(audioClip);
            return AddTrack(audioSource, volume, isMainTrack, label);
        }

        public Guid AddTrack(AudioSource audioSource, float volume = 1.0f, bool isMainTrack = false, string label = default)
        {
            if (audioSource == null)
            {
                throw new ArgumentException("Audio source is empty.", nameof(audioSource));
            }

            if (audioSource.clip.frequency != DefaultFrequency)
            {
                throw new Exception($"Audio clip frequency {audioSource.clip.frequency} is not supported. Only {DefaultFrequency} is supported.");
            }

            if (isMainTrack && tracks.Exists(t => t.IsMain))
            {
                throw new Exception("Main track already exists.");
            }

            audioSource.volume = volume;
            var track = new Track(audioSource, label, isMainTrack);
            tracks.Add(track);

            return track.Id;
        }

        public void SetTrackVolume(Guid id, float volume)
        {
            if (volume > 1f)
            {
                volume = 1f;
            }

            var track = tracks.FirstOrDefault(track => track.Id == id);
            if (track != null)
            {
                track.AudioSource.volume = volume;
            }
        }

        public void PlayTrack(Guid id, long timestamp = 0)
        {
            tracks.FirstOrDefault(track => track.Id == id)?.AudioSource.Play();
        }

        public void PauseTrack(Guid id)
        {
            tracks.FirstOrDefault(track => track.Id == id)?.AudioSource.Pause();
        }

        public void StopTrack(Guid id)
        {
            tracks.FirstOrDefault(track => track.Id == id)?.AudioSource.Stop();
        }

        public void PlayAllTracks()
        {
            tracks.ForEach(track => track.AudioSource.Play());
        }

        public void PauseAllTracks()
        {
            tracks.ForEach(track => track.AudioSource.Pause());
        }

        public void UnPauseAllTracks()
        {
            tracks.ForEach(track => track.AudioSource.UnPause());
        }

        public void StopAllTracks()
        {
            tracks.ForEach(track => track.AudioSource.Stop());
        }

        public async UniTask SaveAsWav(string fileName)
        {
            // Save each track as wav file.
            var cacheFileDir = FileStorageUtility.BaseDirectory;
            var inputReaders = new List<AudioFileReader>();
            var paths = await UniTask.WhenAll(tracks.Select(async (track) =>
            {
                var data = AudioClipData.CreateFromAudioSource(track.AudioSource, true);
                var path = await SaveAudioClipData(data, cacheFileDir);
                inputReaders.Add(new AudioFileReader(path));

                return path;
            }).ToArray());

            // Create sample providers.
            var sampleProviders = inputReaders.Select(reader => CreateSampleProvider(reader));

            // Merge all tracks.
            var mixer = new MixingSampleProvider(sampleProviders);

            await UniTask.RunOnThreadPool(() =>
                {
                    // Save as wav file.
                    WaveFileWriter.CreateWaveFile16(fileName, mixer);
                    foreach (var reader in inputReaders)
                    {
                        reader.Dispose();
                    }

                    // Delete temporary audio files.
                    Array.ForEach(paths, path => File.Delete(path));
                });
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                return;
            }

            if (disposing)
            {
                StopAllTracks();
                if (audioEditorGo != null)
                {
                    UnityEngine.Object.Destroy(audioEditorGo);
                }
            }

            disposedValue = true;
        }

        private UniTask<string> SaveAudioClipData(AudioClipData data, string destPath)
        {
            string WriteClipDataToFile(AudioClipData data)
            {
                try
                {
                    WavUtility.FromAudioClipData(data, destPath, out var filePath);
                    return filePath;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return null;
                }
            }

            return UniTask.RunOnThreadPool(data => WriteClipDataToFile((AudioClipData)data), data, configureAwait: true);
        }

        private UniTask<AudioFileReader> ReadAudioFile(string filePath)
        {
            return UniTask.RunOnThreadPool(filePath => new AudioFileReader((string)filePath), filePath, configureAwait: true);
        }

        private ISampleProvider CreateSampleProvider(AudioFileReader reader)
        {
            bool isMonoClip = reader.WaveFormat.Channels == 1;
            return isMonoClip ?
                new MonoToStereoSampleProvider(reader)
                {
                    LeftVolume = 1.0f,
                    RightVolume = 1.0f,
                }
                : reader;
        }

        private AudioSource CreateAudioSource(AudioClip audioClip)
        {
            if (audioEditorGo == null)
            {
                audioEditorGo = new GameObject(nameof(DefaultAudioEditor));
            }

            var audioSource = audioEditorGo.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.spatialBlend = 0f;
            audioSource.loop = false;

            return audioSource;
        }

        private sealed class Track
        {
            private string label;

            public Track(AudioSource audioSource, string label = default, bool isMain = false)
            {
                if (audioSource == null || audioSource.clip == null)
                {
                    throw new ArgumentException("Audio source or clip is empty.", nameof(audioSource));
                }

                AudioSource = audioSource;
                this.label = label;
                IsMain = isMain;
                Id = Guid.NewGuid();
            }

            public bool IsMain { get; private set; }

            public Guid Id { get; private set; }

            public AudioSource AudioSource { get; private set; }
        }
    }
}
