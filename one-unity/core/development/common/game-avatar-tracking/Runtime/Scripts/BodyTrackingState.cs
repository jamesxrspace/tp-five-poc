using Animancer;
using TPFive.Game.Avatar.HumanPosePlayable;
using UnityEngine.Playables;

namespace TPFive.Game.Avatar.Tracking
{
    public class BodyTrackingState : AnimancerState
    {
        private ScriptPlayable<HumanPosePlayableBehaviour> humanPosePlayable;
        private HumanPosePlayableBehaviour humanPosePlayableBehaviour;
        private float length = -1f;

        public override float Length => length;

        public override AnimancerState Clone(AnimancerPlayable root)
        {
            return new BodyTrackingState();
        }

        public void UpdateHumanPose(ref UnityEngine.HumanPose pose)
        {
            humanPosePlayableBehaviour?.SetHumanPose(ref pose);
        }

        protected override void CreatePlayable(out Playable playable)
        {
            humanPosePlayable = HumanPosePlayableBehaviour.CreatePlayable(Root.Graph);
            humanPosePlayableBehaviour = humanPosePlayable.GetBehaviour();
            length = (float)humanPosePlayable.GetDuration();
            playable = humanPosePlayable;
        }
    }
}