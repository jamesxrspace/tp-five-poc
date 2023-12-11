using System;
using UnityEngine;

namespace TPFive.Game.Video
{
    public interface IVideoPlayer : IHasAliveCheck
    {
        event Action<IVideoPlayer, VideoEventType> VideoPlayerEvent;

        bool IsPlaying { get; }

        string VideoPath { get; }

        double CurrentTimeMs { get; }

        double DurationMs { get; }

        float Volume { get; set; }

        float PlaybackRate { get; set; }

        bool AudioMute { get; set; }

        float AudioMinDistance { get; set; }

        float AudioMaxDistance { get; set; }

        float AudioSpatialBlend { get; set; }

        bool IsLoop { get; set; }

        ContentDisplayType ContentDisplayType { get; set; }

        Texture GetTexture(int index = 0);

        void ApplyMeshRenderer(Renderer renderer, Texture2D defaultTexture);

        void OpenFile(VideoPath videoPath, bool autoPlay);

        void Play();

        void Pause();

        void PlayOrPause();

        void Stop();

        void Seek(float timeMs);

        void SetVideoMapping(string videoMapping);

        void SetStereoPacking(string stereoPacking);
    }
}