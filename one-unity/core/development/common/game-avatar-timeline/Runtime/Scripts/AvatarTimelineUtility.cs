using System;
using System.Collections.Generic;
using TPFive.Game.Avatar.Timeline.AvatarObjectControl;
using TPFive.Game.Avatar.Timeline.OutputWeight;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Timeline
{
    public static class AvatarTimelineUtility
    {
        /// <summary>
        /// For runtime, bind asset to avatar playable components.
        /// </summary>
        /// <param name="avatarTimelinePlayable">The playable components of avatar.</param>
        /// <param name="playableDirector">The timeline player of avatar.</param>
        /// <param name="timelineAsset">The motion asset of avatar.</param>
        public static void BindingToTimelinePlayable(
            IAvatarTimelinePlayable avatarTimelinePlayable,
            PlayableDirector playableDirector,
            TimelineAsset timelineAsset)
        {
            if (!playableDirector.playableGraph.IsValid())
            {
                Debug.LogError("PlayableGraph is not valid, can't bind animator");
                return;
            }

            foreach (var trackAsset in timelineAsset.GetOutputTracks())
            {
                if (trackAsset is AnimationTrack animationTrack)
                {
                    playableDirector.SetGenericBinding(animationTrack, avatarTimelinePlayable.Animator);
                }

                if (trackAsset is AudioTrack audioTrack)
                {
                    playableDirector.SetGenericBinding(audioTrack, avatarTimelinePlayable.AudioSource);
                }
            }
        }

        /// <summary>
        /// For runtime, create behaviours of output control.
        /// </summary>
        /// <param name="playableDirector">The playable director at avatar.</param>
        /// <returns>The behaviours which can control Animation/AudioSource/AvatarControl output weight.</returns>
        public static IAvatarTimelineOutput[] CreateOutputControlBehaviour(PlayableDirector playableDirector)
        {
            var director = playableDirector;

            if (director == null)
            {
                throw new Exception("Can't find PlayableDirector");
            }

            var graph = director.playableGraph;
            if (!graph.IsValid())
            {
                throw new Exception("PlayableGraph is invalid");
            }

            var animationWeightBehaviour = CreateAnimationOutput(graph);
            var audioWeightBehaviour = CreateAudioOutput(graph);
            var avatarControlWeightBehaviour = CreateAvatarControlOutput(graph);

            return new[] { animationWeightBehaviour, audioWeightBehaviour, avatarControlWeightBehaviour };
        }

        private static IAvatarTimelineOutput CreateAnimationOutput(PlayableGraph graph)
        {
            // create weight behaviour
            var playable = ScriptPlayable<AnimationOutputWeightBehaviour>.Create(graph);
            var output = ScriptPlayableOutput.Create(graph, "AnimationOutputWeight");
            output.SetSourcePlayable(playable);
            var behaviour = playable.GetBehaviour();

            // initialize weight behaviour
            behaviour.Weight = 0f;
            var outputCount = graph.GetOutputCountByType<AnimationPlayableOutput>();
            var outputs = new AnimationPlayableOutput[outputCount];
            for (int i = 0; i < outputCount; i++)
            {
                outputs[i] = (AnimationPlayableOutput)graph.GetOutputByType<AnimationPlayableOutput>(i);
            }

            behaviour.SetOutputs(outputs);

            return behaviour;
        }

        private static IAvatarTimelineOutput CreateAudioOutput(PlayableGraph graph)
        {
            // create weight behaviour
            var playable = ScriptPlayable<AudioOutputWeightBehaviour>.Create(graph);
            var output = ScriptPlayableOutput.Create(graph, "AudioOutputWeight");
            output.SetSourcePlayable(playable);
            var behaviour = playable.GetBehaviour();

            // initialize weight behaviour
            behaviour.Weight = 0f;
            var outputCount = graph.GetOutputCountByType<AudioPlayableOutput>();
            var outputs = new AudioPlayableOutput[outputCount];
            for (int i = 0; i < outputCount; i++)
            {
                outputs[i] = (AudioPlayableOutput)graph.GetOutputByType<AudioPlayableOutput>(i);
            }

            behaviour.SetOutputs(outputs);

            return behaviour;
        }

        private static IAvatarTimelineOutput CreateAvatarControlOutput(PlayableGraph graph)
        {
            // create weight behaviour
            var playable = ScriptPlayable<AvatarControlOutputWeightBehaviour>.Create(graph);
            var output = ScriptPlayableOutput.Create(graph, "AvatarControlOutputWeight");
            output.SetSourcePlayable(playable);
            var behaviour = playable.GetBehaviour();

            // initialize weight behaviour
            behaviour.Weight = 0f;
            var outputCount = graph.GetOutputCountByType<ScriptPlayableOutput>();
            var outputs = new List<ScriptPlayableOutput>();
            for (int i = 0; i < outputCount; i++)
            {
                var scriptPlayableOutput = (ScriptPlayableOutput)graph.GetOutputByType<ScriptPlayableOutput>(i);

                if (scriptPlayableOutput.GetReferenceObject() is not AvatarControlTrack)
                {
                    continue;
                }

                outputs.Add(scriptPlayableOutput);
            }

            behaviour.SetOutputs(outputs.ToArray());

            return behaviour;
        }
    }
}