using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    [CreateAssetMenu(menuName = "SocialLobby/Agent/Decision/TargetInRange")]
    public class DecisionTargetInRange : DecisionBase
    {
        [SerializeField]
        private float minDistanceToTagetPos = 0.1f;

        private float? minDistanceToTagetPosSqr;

        public override bool Check(StateController fsm)
        {
            return Vector3.SqrMagnitude(fsm.AvatarRootPosition - fsm.TargetWorldPosition) <
                   (minDistanceToTagetPosSqr ??
                    (minDistanceToTagetPosSqr = minDistanceToTagetPos * minDistanceToTagetPos).Value);
        }
    }
}