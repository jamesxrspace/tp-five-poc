using Animancer;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace TPFive.Game.Avatar.Tracking
{
    public class FaceTrackingState : AnimancerState
    {
        public override float Length => -1;

        public override AnimancerState Clone(AnimancerPlayable root)
        {
            return new FaceTrackingState();
        }

        protected override void CreatePlayable(out Playable playable)
        {
            // The AnimancerLayer should work with a playable.
            // Create an empty playable to represent the face tracking status.
            playable = AnimationMixerPlayable.Create(Root.Graph);
        }
    }
}