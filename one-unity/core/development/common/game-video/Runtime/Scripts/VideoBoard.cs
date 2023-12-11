using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using IVideoService = TPFive.Game.Video.IService;

namespace TPFive.Game.Video
{
    using IVideoService = TPFive.Game.Video.IService;

    /// <summary>
    /// Define this VideoBoard will display the video with fit or stretch in this board's mesh.
    /// </summary>
    public enum ContentDisplayType
    {
        /// <summary>
        /// Content will fit the mesh. Your content display may be distorted on mesh.
        /// </summary>
        Fit,

        /// <summary>
        /// Content will stretch the mesh like the letterbox effect.
        /// Your content display will keep it's ratio on mesh.
        /// </summary>
        Stretch,
    }

    public sealed class VideoBoard : MonoBehaviour
    {
        [SerializeField]
        private string boardName;

        [Header("Render")]
        [SerializeField]
        private Renderer videoRenderer;
        [SerializeField]
        private Texture2D defaultTexture;
        [SerializeField]
        private ContentDisplayType contentDisplayType = ContentDisplayType.Stretch;

        [Header("Audio")]
        [SerializeField]
        [Range(0f, 1f)]
        private float volume = 1f;
        [SerializeField]
        private bool audioMute = false;
        [SerializeField]
        private float audioMinDistance = 0f;
        [SerializeField]
        private float audioMaxDistance = 5f;
        [SerializeField]
        private float audioSpatialBlend = 1f;

        [Header("Playback")]
        [SerializeField]
        private bool autoOpen = false;
        [SerializeField]
        private bool autoPlayOnStart = false;
        [SerializeField]
        private bool isLoop = false;

        [Header("Source")]
        [SerializeField]
        private VideoPath videoPath;

        [Header("Event")]
        [SerializeField]
        private OnReady onReady = new OnReady();

        private IVideoPlayer videoPlayer;
        private IVideoService videoService;

        private bool IsReady => videoPlayer.IsAlive();

        [Inject]
        public void Constructor(IVideoService service)
        {
            videoService = service;
            InitVideoPlayer(destroyCancellationToken).Forget();
        }

        [ContextMenu(nameof(OpenFile))]
        public void OpenFile(bool autoPlay = false)
        {
            if (!IsReady)
            {
                return;
            }

            autoPlayOnStart = autoPlay;
            videoPlayer.OpenFile(videoPath, autoPlay);
        }

        [ContextMenu(nameof(Play))]
        public void Play()
        {
            if (!IsReady)
            {
                return;
            }

            videoPlayer.Play();
        }

        [ContextMenu(nameof(Pause))]
        public void Pause()
        {
            if (!IsReady)
            {
                return;
            }

            videoPlayer.Pause();
        }

        [ContextMenu(nameof(Stop))]
        public void Stop()
        {
            if (!IsReady)
            {
                return;
            }

            videoPlayer.Stop();
        }

        private IEnumerator Start()
        {
            if (autoOpen)
            {
                yield return new WaitUntil(() => IsReady);
                OpenFile(autoPlayOnStart);
            }
        }

        private async UniTaskVoid InitVideoPlayer(CancellationToken token)
        {
            var (cancelled, player) = await videoService.CreateVideoPlayer(transform)
                .AttachExternalCancellation(token)
                .SuppressCancellationThrow();

            if (cancelled)
            {
                if (player.IsAlive())
                {
                    var monoBehaviour = player as MonoBehaviour;
                    if (monoBehaviour != null)
                    {
                        Destroy(monoBehaviour.gameObject);
                    }
                }

                return;
            }

            videoPlayer = player;
            videoPlayer.ApplyMeshRenderer(videoRenderer, defaultTexture);
            videoPlayer.ContentDisplayType = contentDisplayType;
            videoPlayer.Volume = volume;
            videoPlayer.AudioMute = audioMute;
            videoPlayer.AudioMinDistance = audioMinDistance;
            videoPlayer.AudioMaxDistance = audioMaxDistance;
            videoPlayer.AudioSpatialBlend = audioSpatialBlend;
            videoPlayer.IsLoop = isLoop;

            onReady?.Invoke();
        }

        public class OnReady : UnityEvent
        {
        }
    }
}