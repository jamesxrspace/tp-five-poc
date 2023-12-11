using System;
using System.Collections.Generic;
using Animancer;
using TPFive.Game.Avatar.Attachment;
using TPFive.Game.Avatar.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Motion
{
    public class AvatarMotionManager :
        MonoBehaviour,
        IAvatarMotionManager,
        IAvatarTimelinePlayable
    {
        [SerializeField]
        private float fadeDuration = 0.25f;

        private int layerIndex;
        private AnimancerLayer layer;
        private IReadOnlyDictionary<Guid, TimelineAsset> motionDict;
        private Guid currentMotionUid = Guid.Empty;

        public event Action OnStop;

        public AnimancerComponent Animancer { get; set; }

        public int AnimancerLayerIndex
        {
            get => layerIndex;
            set
            {
                layerIndex = value;
                layer = Animancer.Layers[layerIndex];
            }
        }

        public Animator Animator => Animancer.Animator;

        public AudioSource AudioSource { get; set; }

        public IAnchorPointProvider AnchorPointProvider { get; set; }

        public bool IsPlaying => layer.IsAnyStatePlaying();

        public double Time
        {
            get => layer.CurrentState?.Time ?? -1d;
            set
            {
                if (layer.CurrentState == null)
                {
                    Debug.LogWarning("CurrentState is null.");
                    return;
                }

                layer.CurrentState.Time = (float)value;
            }
        }

        public double Duration => layer.CurrentState?.Length ?? -1d;

        public float Weight
        {
            get => layer.Weight;
            set => layer.Weight = value;
        }

        public IMotionItem[] Motions { get; private set; }

        public int MotionCount => motionDict.Count;

        public Guid CurrentMotionUid => currentMotionUid;

        public void Play(Guid uid, bool loop = false)
        {
            if (!motionDict.TryGetValue(uid, out var asset))
            {
                Debug.LogWarning($"Motion {uid} not found.");

                return;
            }

            PlayTimelineAsset(asset, loop);
            currentMotionUid = uid;
        }

        public void Play(TimelineAsset asset, bool loop = false)
        {
            if (asset == null)
            {
                Debug.LogWarning("TimelineAsset is null.");

                return;
            }

            PlayTimelineAsset(asset, loop);
            currentMotionUid = Guid.Empty;
        }

        public void SetAvatarMotionCategory(AvatarMotionCategory category)
        {
            motionDict = category.GetMotionDict();
            Motions = category.Motions;
        }

        public TimelineAsset GetMotion(Guid uid)
        {
            if (!motionDict.TryGetValue(uid, out var motion))
            {
                Debug.LogWarning($"Motion {uid} not found.");

                return null;
            }

            return motion;
        }

        public void Stop()
        {
            layer.StartFade(0f, fadeDuration);
            currentMotionUid = Guid.Empty;
            OnStop?.Invoke();
        }

        private void PlayTimelineAsset(TimelineAsset asset, bool loop = false)
        {
            if (IsPlaying)
            {
                OnStop?.Invoke();
            }

            var transition = new PlayableAssetTransition
            {
                Asset = asset,
                FadeDuration = fadeDuration,
            };

            var state = layer.Play(transition);
            state.Events.OnEnd = () =>
            {
                if (loop)
                {
                    state.Time = 0f;
                    return;
                }

                Stop();
            };
        }
    }
}