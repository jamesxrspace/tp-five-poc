using System;
using UnityEngine;

namespace TPFive.Game.Record
{
    public class AudioClipData
    {
        public AudioClipData()
        {
        }

        public AudioClipData(float[] data, int channelCount, int frequency, string name = default)
        {
            Name = name ?? Guid.NewGuid().ToString();
            Buffer = data;
            Samples = data.Length / channelCount;
            Channels = channelCount;
            Frequency = frequency;
            LengthSec = Samples / (float)Frequency;
        }

        public string Name { get; private set; }

        public float[] Buffer { get; private set; }

        public int Samples { get; private set; }

        public int Channels { get; private set; }

        public int Frequency { get; private set; }

        public float LengthSec { get; private set; }

        public static AudioClipData CreateFromAudioSource(AudioSource audioSource, bool rewriteDecibel = true)
        {
            var data = CreateFromAudioClip(audioSource.clip);
            if (rewriteDecibel)
            {
                data.RewriteDecibel(audioSource.volume);
            }

            return data;
        }

        public static AudioClipData CreateFromAudioClip(AudioClip audioClip)
        {
            var data = new AudioClipData
            {
                Name = audioClip.name,
                Samples = audioClip.samples,
                Channels = audioClip.channels,
                Frequency = audioClip.frequency,
                Buffer = new float[audioClip.samples * audioClip.channels],
                LengthSec = audioClip.length,
            };
            audioClip.GetData(data.Buffer, 0);
            return data;
        }

        public static AudioClip CreateAudioClip(AudioClipData data)
        {
            AudioClip clip = AudioClip.Create(data.Name, data.Samples, data.Channels, data.Frequency, false);
            clip.SetData(data.Buffer, 0);
            return clip;
        }

        public void RewriteDecibel(float multiplier)
        {
            if (multiplier < 0 || multiplier >= 1f)
            {
                return;
            }

            for (int i = 0; i < Buffer.Length; ++i)
            {
                Buffer[i] = Mathf.Clamp(Buffer[i] * multiplier, -1f, 1f);
            }
        }
    }
}
