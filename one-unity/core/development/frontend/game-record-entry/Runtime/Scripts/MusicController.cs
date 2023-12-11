using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record.Entry
{
    public class MusicController : IDisposable
    {
        private readonly ILogger log;
        private readonly Dictionary<string, AudioSource> audioSources = new ();
        private bool disposed = false;

        public MusicController(
            ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger<MusicController>();
        }

        ~MusicController()
        {
            Dispose(false);
        }

        public AudioSource Upsert(string name, AudioClip audioClip)
        {
            if (audioSources.TryGetValue(name, out var audioSource))
            {
                audioSource.clip = audioClip;
                log.LogDebug("Get AudioSource {name} and updated with new AudioClip : {audioClip}", name, audioClip ? audioClip.name : null);
                return audioSource;
            }

            log.LogDebug("Create AudioSource {name} with AudioClip : {audio}", name, audioClip ? audioClip.name : null);
            return AddAudioSourceToDictionary(name, audioClip);
        }

        public void DestroyAudioSource(string name)
        {
            var audioSource = Find(name);

            if (audioSource == null)
            {
                return;
            }

            UnityEngine.Object.Destroy(audioSource.gameObject);
            audioSources.Remove(name);

            log.LogDebug("Destroy AudioSource {name} successfully", name);
        }

        public AudioSource Find(string name)
        {
            audioSources.TryGetValue(name, out var audioSource);

            return audioSource;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    foreach (var audioSource in audioSources.Values)
                    {
                        UnityEngine.Object.Destroy(audioSource.gameObject);
                    }

                    audioSources.Clear();
                }

                disposed = true;
            }
        }

        private AudioSource AddAudioSourceToDictionary(string name, AudioClip audioClip)
        {
            var audioSource = new GameObject(name).AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f;
            audioSource.loop = false;
            audioSource.volume = 1f;
            audioSource.clip = audioClip;
            audioSources.Add(name, audioSource);
            log.LogDebug("Add AudioSource {name} to Dict successfully", name);
            return audioSource;
        }
    }
}
