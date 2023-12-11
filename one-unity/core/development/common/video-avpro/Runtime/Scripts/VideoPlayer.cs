using System;
using Microsoft.Extensions.Logging;
using RenderHeads.Media.AVProVideo;
using TPFive.Extended.Video.AVPro.Extension;
using TPFive.Game.Extensions;
using TPFive.Game.Video;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.Video.AVPro
{
    [RequireComponent(typeof(MediaPlayer), typeof(AudioOutput), typeof(ApplyToMesh))]
    public sealed partial class VideoPlayer : MonoBehaviour, IVideoPlayer
    {
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private MediaPlayer mediaPlayer;
        [SerializeField]
        private AudioOutput audioOutput;
        [SerializeField]
        private ApplyToMesh applyToMesh;
        [SerializeField]
        private ContentDisplayType contentDisplayType = ContentDisplayType.Stretch;

        private ILogger _logger;
        private bool _autoPlay;
        private bool _isLiveStreaming;
        private VideoPath _videoPath = new VideoPath();
        private bool _isReady = false;

        public event Action<IVideoPlayer, VideoEventType> VideoPlayerEvent;

        public bool IsPlaying => HasControl && mediaPlayer.Control.IsPlaying();

        public string VideoPath => _videoPath.GetResolvedFilePath();

        public double CurrentTimeMs => HasControl ? mediaPlayer.Control.GetCurrentTime() * 1000d : 0d;

        public bool HasControl => mediaPlayer.Control != null;

        public IMediaInfo MediaPlayerInfo => mediaPlayer.Info;

        public double DurationMs
        {
            get
            {
                if (mediaPlayer.Info != null)
                {
                    // [Workaround] On Android & iOS, youtube hls duration is not infinity
                    if (_isLiveStreaming)
                    {
                        return double.PositiveInfinity;
                    }

                    return mediaPlayer.Info.GetDuration() * 1000d;
                }

                return 0d;
            }
        }

        public float Volume
        {
            get => HasControl ? mediaPlayer.Control.GetVolume() : 0f;

            set
            {
                if (HasControl)
                {
                    mediaPlayer.Control.SetVolume(value);
                }
            }
        }

        public float PlaybackRate
        {
            get => HasControl ? mediaPlayer.Control.GetPlaybackRate() : 0f;

            set
            {
                if (HasControl)
                {
                    mediaPlayer.Control.SetPlaybackRate(value);
                }
            }
        }

        public bool AudioMute
        {
            get => mediaPlayer.AudioMuted;
            set => mediaPlayer.AudioMuted = value;
        }

        public float AudioMinDistance
        {
            get => audioSource.minDistance;
            set => audioSource.minDistance = value;
        }

        public float AudioMaxDistance
        {
            get => audioSource.maxDistance;
            set => audioSource.maxDistance = value;
        }

        public float AudioSpatialBlend
        {
            get => audioSource.spatialBlend;
            set => audioSource.spatialBlend = value;
        }

        public bool IsLoop
        {
            get => mediaPlayer.Loop;
            set => mediaPlayer.Loop = value;
        }

        public ContentDisplayType ContentDisplayType
        {
            get => contentDisplayType;
            set
            {
                contentDisplayType = value;

                // It may could be thumbnail or video texture.
                if (TryGetCurrentRenderTextureSize(out var textureSize))
                {
                    ApplyDisplayType(
                        value,
                        textureSize,
                        applyToMesh.MeshRenderer.transform.localScale);
                }
            }
        }

        public bool IsReady => _isReady;

        [Inject]
        public void Construct(ILoggerFactory loggerFactory)
        {
            _logger = Game.Logging.Utility.CreateLogger<VideoPlayer>(loggerFactory);
        }

        public Texture GetTexture(int index = 0)
        {
            return IsAlive(mediaPlayer.TextureProducer) ? mediaPlayer.TextureProducer.GetTexture(index) : null;
        }

        public void ApplyMeshRenderer(Renderer renderer, Texture2D defaultTexture)
        {
            applyToMesh.MeshRenderer = renderer;
            applyToMesh.DefaultTexture = defaultTexture;

            if (defaultTexture != null)
            {
                ApplyDisplayType(
                    contentDisplayType,
                    defaultTexture.Size(),
                    applyToMesh.MeshRenderer.transform.localScale);
            }
        }

        public void OpenFile(VideoPath videoPath, bool autoPlay)
        {
            _autoPlay = autoPlay;
            _videoPath = videoPath;

            mediaPlayer.OpenMedia((MediaPathType)videoPath.PathType, videoPath.Path, _autoPlay);
        }

        public void Play()
        {
            mediaPlayer.Play();
        }

        public void Pause()
        {
            _autoPlay = false;

            if (HasControl && mediaPlayer.Control.IsPlaying())
            {
                mediaPlayer.Pause();
            }
        }

        public void PlayOrPause()
        {
            if (HasControl && mediaPlayer.Control.IsPlaying())
            {
                mediaPlayer.Pause();
            }
            else
            {
                mediaPlayer.Play();
            }
        }

        public void Stop()
        {
            _autoPlay = false;

            if (HasControl)
            {
                mediaPlayer.Control.Seek(0);
            }

            mediaPlayer.Stop();
        }

        public void Seek(float timeMs)
        {
            if (!HasControl)
            {
                return;
            }

            double seekProgress = timeMs;
            var mediaDuration = DurationMs;
            if (mediaDuration == 0d || double.IsInfinity(mediaDuration))
            {
                return;
            }

            if (mediaPlayer.Loop)
            {
                seekProgress = seekProgress % mediaDuration;
            }
            else if (seekProgress > mediaDuration)
            {
                seekProgress = mediaDuration;
            }

            mediaPlayer.Control.Seek(seekProgress / 1000d);
        }

        public void SetVideoMapping(string videoMapping)
        {
            if (mediaPlayer == null)
            {
                _logger.LogWarning("SetVideoMapping fail : mediaPlayer is not exist.");
                return;
            }

            if (!Enum.TryParse<VideoMapping>(videoMapping, out var mapping))
            {
                _logger.LogWarning("SetVideoMapping fail : Parse enum is not found {VideoMapping}", videoMapping);
                return;
            }

            mediaPlayer.VideoLayoutMapping = mapping;
        }

        public void SetStereoPacking(string stereoPacking)
        {
            if (mediaPlayer == null)
            {
                _logger.LogWarning("SetStereoPacking fail : mediaPlayer is not exist.");
                return;
            }

            if (!Enum.TryParse<StereoPacking>(stereoPacking, out var stereo))
            {
                _logger.LogWarning("SetStereoPacking fail : Parse enum is not found {StereoPacking}", stereoPacking);
                return;
            }

            var hints = mediaPlayer.FallbackMediaHints;
            hints.stereoPacking = stereo;
            mediaPlayer.FallbackMediaHints = hints;
        }

        public void CloseVideo()
        {
            _autoPlay = false;
            _videoPath = new VideoPath();

            mediaPlayer.Stop();
            mediaPlayer.CloseMedia();
            _isLiveStreaming = false;
        }

        public void SetAudioMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        public void ClearVideoPlayer()
        {
            _autoPlay = false;
            _videoPath = new VideoPath();

            VideoPlayerEvent = null;
            ResetRenderSetting();
            mediaPlayer.Stop();
            mediaPlayer.CloseMedia();
            _isLiveStreaming = false;
        }

        public void SetMediaPlayerToDisplayUGUI(DisplayUGUI displayUGUI)
        {
            displayUGUI.CurrentMediaPlayer = mediaPlayer;
        }

        private static bool IsAlive<T>(T obj)
        {
            if (obj is UnityEngine.Object o)
            {
                return o != null;
            }

            return obj != null;
        }

        private void Awake()
        {
            audioSource.spatialBlend = 1;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            mediaPlayer.AutoOpen = false;
            mediaPlayer.AutoStart = false;
            mediaPlayer.Loop = true;

            // Fixes the issue of incorrect video colors on iPhones.
#if UNITY_IPHONE
            mediaPlayer.PlatformOptionsIOS.textureFormat = MediaPlayer.OptionsApple.TextureFormat.BGRA;
#endif
            audioOutput.ChangeMediaPlayer(mediaPlayer);
            applyToMesh.Player = mediaPlayer;

            mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
        }

        private void Start()
        {
            _isReady = true;
        }

        private void OnDestroy()
        {
            _autoPlay = false;
            _videoPath = null;
            _isLiveStreaming = false;
            if (mediaPlayer != null)
            {
                mediaPlayer.Events.RemoveListener(OnMediaPlayerEvent);
                mediaPlayer.CloseMedia();
            }
        }

        private void OnMediaPlayerEvent(MediaPlayer mediaPlayer, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
        {
            if (mediaPlayer != this.mediaPlayer)
            {
                return;
            }

            if (errorCode != ErrorCode.None)
            {
                _logger.LogWarning("{Name} mediaPlayerEvent ErrorCode : {ErrorCode}", gameObject.name, errorCode);
                return;
            }

            if (eventType == MediaPlayerEvent.EventType.FinishedPlaying)
            {
                if (this.mediaPlayer.Info.GetDuration() == 0d)
                {
                    // [AVPro]
                    // Live streaming video pause/resume (close/reopen) sometimes duration will return zero at first.
                    // On Windows platform this will cause FinishedPlaying event fired, but the video is not finished actually.
                    // Therefore, we ignore the event in this situation.
                    _logger.LogWarning("Ignore HLS video FinishedPlaying event on Windows platform when duration is zero.");
                    return;
                }
            }

            if (eventType == MediaPlayerEvent.EventType.ResolutionChanged)
            {
                var textureSize = new Vector2(mediaPlayer.Info.GetVideoWidth(), mediaPlayer.Info.GetVideoHeight());
                var targetSize = applyToMesh.MeshRenderer.transform.localScale;
                ApplyDisplayType(contentDisplayType, textureSize, targetSize);
            }

            VideoPlayerEvent?.Invoke(this, (VideoEventType)eventType);
        }

        private void ResetRenderSetting()
        {
            if (mediaPlayer != null)
            {
                var hints = mediaPlayer.FallbackMediaHints;
                hints.stereoPacking = StereoPacking.None;
                mediaPlayer.FallbackMediaHints = hints;

                mediaPlayer.VideoLayoutMapping = VideoMapping.Normal;
            }
        }

        private void ApplyDisplayType(ContentDisplayType type, Vector2 contentSize, Vector2 targetSize)
        {
            // Both of Vector2.one is ContentDisplayType.Fit
            if (type == ContentDisplayType.Fit)
            {
                contentSize = Vector2.one;
                targetSize = Vector2.one;
            }

            // if not ContentDisplayType.Fit, then apply letterbox that equal (ContentDisplayType.Stretch)
            applyToMesh.ApplyLetterbox(contentSize, targetSize);
            applyToMesh.ForceUpdate();
        }

        private bool TryGetCurrentRenderTextureSize(out Vector2 size)
        {
            size = Vector2.one;

            if (mediaPlayer.Info != null)
            {
                size = new Vector2(mediaPlayer.Info.GetVideoWidth(), mediaPlayer.Info.GetVideoHeight());

                // if current video info is zero is not allow.
                // When video is not start, the video info is zero.
                return size != Vector2.zero;
            }

            if (applyToMesh.MeshRenderer != null
                && applyToMesh.MeshRenderer.material != null
                && applyToMesh.MeshRenderer.material.mainTexture != null)
            {
                size = applyToMesh.MeshRenderer.material.mainTexture.Size();
                return true;
            }

            if (applyToMesh.DefaultTexture != null)
            {
                size = applyToMesh.DefaultTexture.Size();
                return true;
            }

            _logger.LogDebug("{MethodName} fail : No Any texture is not exist.", nameof(TryGetCurrentRenderTextureSize));
            return false;
        }
    }
}