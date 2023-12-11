using TPFive.Game.Avatar.Attachment;
using UnityEngine;

namespace TPFive.Game.Avatar.Timeline
{
    public interface IAvatarTimelinePlayable
    {
        // The animator at avatar.
        Animator Animator { get; }

        // The audio source at avatar.
        AudioSource AudioSource { get; }

        // The avatar attachment at avatar.
        IAnchorPointProvider AnchorPointProvider { get; }
    }
}