using System;
using TPFive.Game.Avatar.Attachment;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Timeline
{
    public sealed class AvatarTimelineManager :
        MonoBehaviour,
        IAvatarTimelineManager,
        IAvatarTimelinePlayable
    {
        [SerializeField]
        private PlayableDirector playableDirector;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private float weight;

        private IAvatarTimelineOutput[] timelineOutputs;

        public event Action OnStop;

        public bool IsPlaying => playableDirector != null && playableDirector.state == PlayState.Playing;

        public double Time
        {
            get => playableDirector == null ? 0d : playableDirector.time;
            set
            {
                if (playableDirector == null)
                {
                    return;
                }

                playableDirector.time = value;
            }
        }

        public double Duration => playableDirector == null ? 0d : playableDirector.duration;

        public float Weight
        {
            get => weight;
            set => SetMotionWeight(value);
        }

        public PlayableDirector PlayableDirector
        {
            get => playableDirector;
            set
            {
                if (playableDirector == value)
                {
                    return;
                }

                if (playableDirector != null)
                {
                    playableDirector.stopped -= OnPlayableDirectorStopped;
                }

                playableDirector = value;

                if (playableDirector != null)
                {
                    playableDirector.stopped += OnPlayableDirectorStopped;
                }
            }
        }

        public Animator Animator
        {
            get => animator;
            set => animator = value;
        }

        public AudioSource AudioSource
        {
            get => audioSource;
            set => audioSource = value;
        }

        public IAnchorPointProvider AnchorPointProvider { get; set; }

        public void Play(TimelineAsset asset, bool loop = false)
        {
            if (playableDirector == null)
            {
                return;
            }

            if (IsPlaying)
            {
                OnStop?.Invoke();
            }

            playableDirector.extrapolationMode = loop ? DirectorWrapMode.Loop : DirectorWrapMode.None;
            PlayableDirector.playableAsset = asset;
            PlayableDirector.RebuildGraph();
            timelineOutputs = AvatarTimelineUtility.CreateOutputControlBehaviour(PlayableDirector);
            AvatarTimelineUtility.BindingToTimelinePlayable(this, playableDirector, asset);
            PlayableDirector.Play();
        }

        public void Stop()
        {
            if (PlayableDirector == null)
            {
                return;
            }

            PlayableDirector.Stop();
        }

        private void Awake()
        {
            if (playableDirector == null)
            {
                playableDirector = GetComponent<PlayableDirector>();
            }
        }

        private void OnDestroy()
        {
            if (playableDirector != null)
            {
                playableDirector.stopped -= OnPlayableDirectorStopped;
            }
        }

        private void SetMotionWeight(float weight)
        {
            this.weight = weight;

            if (timelineOutputs == null || timelineOutputs.Length == 0f)
            {
                return;
            }

            foreach (var timelineOutput in timelineOutputs)
            {
                timelineOutput.Weight = weight;
            }
        }

        private void OnPlayableDirectorStopped(PlayableDirector director)
        {
            OnStop?.Invoke();
        }
    }
}