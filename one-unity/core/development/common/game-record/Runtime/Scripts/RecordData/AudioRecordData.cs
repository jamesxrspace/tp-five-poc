using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.Record
{
    // TBD: [TF3R-122] [Unity] record data should derive to framesbasedata & non-framebaedata
    public class AudioRecordData : RecordData
    {
        private int startPosition = -1;
        private int endPosition = -1;

        // TBD: we store the start & end poistion of the audio clip in profile.
        public AudioRecordData(string id = default, int frequency = 44100)
            : base(id: id)
        {
            Frequency = frequency;
        }

        public AudioSource AudioSource { get; set; }

        public AudioClipData AudioClipData { get; set; }

        public float Volume { get; private set; } = 1f;

        public int Frequency { get; } = 44100;

        public override RecordDataType GetType()
        {
            return RecordDataType.Audio;
        }

        public void Trim(int startPosition, int endPosition)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
        }

        public void Trim(float startTime, float endTime)
        {
            if (AudioSource.clip != null)
            {
                var frequency = AudioSource.clip.frequency;
                startPosition = (int)(startTime * frequency);
                endPosition = (int)(Math.Min(endTime, AudioSource.clip.length) * frequency);
            }
        }

        public void DoTrim()
        {
            if (AudioSource.clip == null)
            {
                return;
            }

            AudioClipData = (startPosition == -1)
               ? AudioClipData.CreateFromAudioClip(AudioSource.clip)
               : new AudioClipData(
                    WavUtility.TrimmedAudioClip(AudioSource.clip, startPosition, endPosition),
                    AudioSource.clip.channels,
                    AudioSource.clip.frequency,
                    AudioSource.clip.name);

            if (AudioSource != null)
            {
                SetupAudioSourceByClipData(AudioSource);
            }

            startPosition = -1;
            endPosition = -1;
        }

        // Calls before record session, to initial things we need while recording.
        public virtual bool BeforeRecord()
        {
            if (AudioSource.clip == null)
            {
                throw new Exception("AudioClip not found");
            }

            if (AudioSource.clip.frequency != Frequency)
            {
                // TBD: [TF3R-140] [Unity][Optimize] resample all AudioClip to same frequency before file save.
                throw new Exception($"AudioClip frequency {AudioSource.clip.frequency} does not match {Frequency}");
            }

            return true;
        }

        // Calls after record session was done, to release resource occupied by record object
        // , which are not needed after record session.
        public virtual void AfterRecord()
        {
        }

        public void Bind(AudioSource target, string audioFormat = default)
        {
            AudioSource = target;
            Volume = target.volume;

            if (audioFormat != default)
            {
                SetProfile(new Dictionary<string, object>()
                {
                    { "format", Base64.Encode(audioFormat) },
                    { "volume", Volume },
                });
            }

            if (AudioSource != null)
            {
                if (AudioSource.clip == null)
                {
                    if (AudioClipData != null)
                    {
                        AudioSource.clip = AudioClipData.CreateAudioClip(AudioClipData);
                        AudioSource.spatialBlend = 0f;
                        AudioSource.loop = false;
                    }
                }
                else
                {
                    AudioClipData = AudioClipData.CreateFromAudioClip(AudioSource.clip);
                }
            }
        }

        public override void Deserialize(byte[] buffer)
        {
            var byteList = Segment.DeserializeList(buffer);
            base.Deserialize(byteList[0]);
            AudioClipData = WavUtility.ToAudioClipData(byteList[1]);

            Volume = Profile.ContainsKey("volume") ? (float)Profile["volume"] : 1f;
        }

        public override void PostDeserialize()
        {
            AudioSource audioSource = new GameObject(Id).AddComponent<AudioSource>();
            SetupAudioSourceByClipData(audioSource);
            Bind(audioSource);
        }

        public override byte[] Serialize()
        {
            return Segment.SerializeList(new List<byte[]>()
            {
                base.Serialize(),
                WavUtility.FromAudioClipData(AudioClipData),
            });
        }

        private void SetupAudioSourceByClipData(AudioSource audioSource)
        {
            audioSource.clip = AudioClipData.CreateAudioClip(AudioClipData);
            audioSource.spatialBlend = 0f;
            audioSource.loop = false;
            audioSource.volume = Volume;
        }
    }
}
