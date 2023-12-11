using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    [CreateAssetMenu(menuName = "SocialLobby/Agent/Action/MoveToRandomPositionInCircle")]
    public class ActionMoveToRandomPositionInCircle : ActionBase
    {
        [SerializeField]
        private float maxMoveSpeed = 0.2f;
        [SerializeField]
        private float randomPointInCircleRadius = 0.1f;

        public override void Enter(StateController fsm)
        {
            var targetPos = GetRandomPointInCircle(randomPointInCircleRadius);
            fsm.SetTargetLocalPositionXZPlane(targetPos.x, targetPos.z);
        }

        public override void Execute(StateController fsm)
        {
            fsm.AvatarProxy.MoveTo(fsm.TargetWorldPosition, maxMoveSpeed);
        }

        public override void Exit(StateController fsm)
        {
            fsm.SetTargetLocalPositionXZPlane(0, 0);
            fsm.AvatarProxy.StopMove();
        }

        private Vector3 GetRandomPointInCircle(float randomPointInCircleRadius)
        {
            return Random.insideUnitSphere * randomPointInCircleRadius;
        }
    }
}