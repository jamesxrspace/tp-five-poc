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
    public interface IAvatarContextProvider : IHasAliveCheck
    {
        GameObject Root { get; }

        AvatarController Controller { get; }

        Animator Animator { get; }

        AnimancerComponent Animancer { get; set; }

        AvatarFormat AvatarFormat { get; }

        SkinManager SkinManager { get; }

        IAnchorPointProvider AnchorPointProvider { get; }

        IAvatarSitManager SitManager { get; }

        IAvatarTalkManager TalkManager { get; }

        IAvatarTrackingManager TrackingManager { get; }

        IAvatarMotionManager MotionManager { get; }

        IHumanPoseSynchronizer HumanPoseSynchronizer { get; }
    }
}
