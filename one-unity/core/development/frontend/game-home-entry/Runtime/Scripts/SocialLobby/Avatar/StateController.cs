using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public class StateController : MonoBehaviour
    {
        [SerializeField]
        private EntityAvatarProxy avatarProxy;
        [SerializeField]
        private State startState;
        [SerializeField]
        private State currentState;
        [SerializeField]
        private float stateTimeElapsed;
        [SerializeField]
        private Transform pivotTransform;

        private Vector3? targetLocalPosition;

        public float StateTimeElapsed => stateTimeElapsed;

        public Vector3 AvatarRootPosition => avatarProxy.Root.position;

        public EntityAvatarProxy AvatarProxy => avatarProxy;

        public Vector3 PivotPosition => pivotTransform.position;

        /// <summary>
        /// Gets the target position in world space.
        /// </summary>
        /// <value>Pivot.position + TargetLocalPosition.</value>
        public Vector3 TargetWorldPosition
        {
            get
            {
                if (!targetLocalPosition.HasValue)
                {
                    return pivotTransform.position;
                }

                return pivotTransform.position + targetLocalPosition.Value;
            }
        }

        public void Initialize(GameObject avatarGo)
        {
            avatarProxy.Initialize(avatarGo);
        }

        public void Execute(float deltaTime)
        {
            if (currentState == null)
            {
                ChangeState(startState);
            }

            stateTimeElapsed += deltaTime;

            currentState.Execute(this);

            avatarProxy.Execute(deltaTime);
        }

        public void ChangeState(State newState)
        {
            if (currentState == newState)
            {
                return;
            }

            if (newState == null)
            {
                return;
            }

            if (currentState != null)
            {
                currentState.Exit(this);
            }

            currentState = newState;

            currentState.Enter(this);

            stateTimeElapsed = 0;
        }

        public void SetTargetLocalPositionXZPlane(float x, float z)
        {
            targetLocalPosition = new Vector3(x, avatarProxy.Root.position.y, z);
        }
    }
}