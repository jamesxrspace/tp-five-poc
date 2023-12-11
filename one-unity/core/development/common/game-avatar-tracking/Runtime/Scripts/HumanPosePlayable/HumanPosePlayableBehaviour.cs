using UnityEngine.Animations;
using UnityEngine.Playables;

namespace TPFive.Game.Avatar.HumanPosePlayable
{
    public class HumanPosePlayableBehaviour : PlayableBehaviour
    {
        private static readonly int PlayableInputCount = 1;
        private PlayableGraph playableGraph;
        private HumanPoseAnimationJob humanPoseAnimationJob;
        private AnimationScriptPlayable animationScriptPlayable;
        private UnityEngine.HumanPose sourceHumanPose;
        private bool hasSetHumanPose;

        public static ScriptPlayable<HumanPosePlayableBehaviour> CreatePlayable(PlayableGraph playableGraph)
        {
            return ScriptPlayable<HumanPosePlayableBehaviour>.Create(playableGraph, PlayableInputCount);
        }

        public void SetHumanPose(ref UnityEngine.HumanPose humanPose)
        {
            sourceHumanPose = humanPose;
            hasSetHumanPose = true;
        }

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);

            playableGraph = playable.GetGraph();
            humanPoseAnimationJob = HumanPoseAnimationJob.Create();
            animationScriptPlayable = AnimationScriptPlayable.Create(playableGraph, humanPoseAnimationJob);
            playable.ConnectInput(0, animationScriptPlayable, 0);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);

            humanPoseAnimationJob.Dispose();
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);

            // avoid updating the human pose before the human pose has been set.
            if (!hasSetHumanPose)
            {
                return;
            }

            var jobData = animationScriptPlayable.GetJobData<HumanPoseAnimationJob>();
            jobData.SetHumanPose(sourceHumanPose.bodyPosition, sourceHumanPose.bodyRotation, sourceHumanPose.muscles);
            animationScriptPlayable.SetJobData(jobData);
        }
    }
}