using System;
using System.Linq;

namespace TPFive.Game.Record
{
    using Microsoft.Extensions.Logging;
    using UnityEngine.Assertions;

    public class AudioPlaySession : PlaySession
    {
        private IAudioEditor audioEditor;

        private float duration;

        public AudioPlaySession(ILogger logger)
            : base(logger)
        {
        }

        public override void Setup(RecordData[] data)
        {
            Assert.IsTrue(Machine.State == State.StandBy);

            if (audioEditor != null)
            {
                audioEditor?.Dispose();
            }

            audioEditor = new DefaultAudioEditor();

            foreach (var (item, i) in data.OfType<AudioRecordData>().Select((value, i) => (value, i)).ToArray())
            {
                // Setup Track
                audioEditor.AddTrack(item.AudioSource, volume: item.Volume, label: item.Id, isMainTrack: i == 0);

                // Get duration
                var audioClip = item.AudioSource.clip;
                if (audioClip != null)
                {
                    duration = Math.Max(duration, audioClip.length);
                }
            }
        }

        public override void Start()
        {
            base.Start();
            audioEditor.PlayAllTracks();
        }

        public override void Stop()
        {
            base.Stop();
            audioEditor.StopAllTracks();
        }

        public override void Pause()
        {
            base.Pause();
            audioEditor.PauseAllTracks();
        }

        public override void Resume()
        {
            base.Resume();
            audioEditor.UnPauseAllTracks();
        }

        public override float GetDuration()
        {
            return duration;
        }

        public override void Dispose()
        {
            audioEditor?.Dispose();
            audioEditor = null;
        }
    }
}