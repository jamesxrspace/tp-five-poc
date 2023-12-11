using Animancer;
using TPFive.Game.Avatar.Attachment;
using TPFive.Game.Avatar.Motion;
using TPFive.Game.Avatar.Sit;
using TPFive.Game.Avatar.Talk;
using TPFive.Game.Avatar.Tracking;
using UnityEngine;
using XR.Avatar;

namespace TPFive.Game.Avatar
{
    public sealed class AvatarContextProvider : MonoBehaviour, IAvatarContextProvider
    {
        public GameObject Root { get; private set; }

        public AvatarController Controller { get; set; }

        public Animator Animator { get; set; }

        public AnimancerComponent Animancer { get; set; }

        public AvatarFormat AvatarFormat { get; set; }

        public SkinManager SkinManager { get; set; }

        public IAnchorPointProvider AnchorPointProvider { get; set; }

        public IAvatarSitManager SitManager { get; set; }

        public IAvatarTalkManager TalkManager { get; set; }

        public IAvatarMotionManager MotionManager { get; set; }

        public IAvatarTrackingManager TrackingManager { get; set; }

        public IHumanPoseSynchronizer HumanPoseSynchronizer { get; set; }

        private void Awake()
        {
            Root = this.gameObject;
        }

        private void OnDestroy()
        {
            SitManager?.Dispose();
            TalkManager?.Dispose();
            TrackingManager?.Dispose();
            HumanPoseSynchronizer?.Dispose();
        }
    }
}
