using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public class AgentAvatar : MonoBehaviour
    {
        [SerializeField]
        private StateController stateController;

        private GameObject targetAvatarGo;

        public bool CanRun => targetAvatarGo != null;

        public bool IsRunning { get; private set; }

        public void Initialize(GameObject avatarGo)
        {
            targetAvatarGo = avatarGo;
            stateController.Initialize(avatarGo);
        }

        public void Run()
        {
            if (!CanRun)
            {
                return;
            }

            IsRunning = true;
        }

        public void Shutdown()
        {
            IsRunning = false;
        }

        protected void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            stateController.Execute(Time.deltaTime);
        }
    }
}