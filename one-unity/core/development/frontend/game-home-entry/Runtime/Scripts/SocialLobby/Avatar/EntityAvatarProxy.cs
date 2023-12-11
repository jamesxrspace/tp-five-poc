using EasyCharacterMovement;
using TPFive.Game.Avatar;
using UnityEngine;
using UnityEngine.Assertions;

namespace TPFive.Home.Entry.SocialLobby
{
    public sealed class EntityAvatarProxy : MonoBehaviour
    {
        /// <summary>
        /// Indicates offset of normalized timing in our running animation,
        /// when one leg passes the other at the normalized clip times of 0.0 and 0.5.
        /// ref: CharacterNetworkController.cs.
        /// </summary>
        [SerializeField]
        private float normalizedRunningCycleOffset = 0.2f;
        [SerializeField]
        private Character character;
        [SerializeField]
        private bool canRun;
        private ECMAnimatorCommunicator communicator;
        private IAvatarContextProvider avatarContextProvider;

        public Transform Root { get; private set; }

        public void Initialize(GameObject avatarGo)
        {
            Assert.IsNotNull(avatarGo, $"{nameof(EntityAvatarProxy)}.{nameof(Initialize)} failed, character == null");

            Assert.IsNotNull(character, $"{nameof(EntityAvatarProxy)}.{nameof(Initialize)} failed, character == null");

            Root = avatarGo.transform;

            avatarContextProvider =
                avatarGo.GetComponent<IAvatarContextProvider>();
            Assert.IsNotNull(
                avatarContextProvider,
                $"{nameof(EntityAvatarProxy)}.{nameof(Initialize)} failed, avatarContextProvider == null");

            communicator = new ECMAnimatorCommunicator(
                character,
                transform,
                avatarContextProvider.Animator,
                normalizedRunningCycleOffset);
            communicator.CanRun = canRun;
        }

        public void MoveTo(Vector3 worldPosition, float speed)
        {
            var direction = worldPosition - character.transform.position;

            character.SetMovementDirection(direction.normalized * speed);
        }

        public void Execute(float deltaTime)
        {
            communicator?.Update();
        }

        public void StopMove()
        {
            character.SetMovementDirection(Vector3.zero);
        }
    }
}