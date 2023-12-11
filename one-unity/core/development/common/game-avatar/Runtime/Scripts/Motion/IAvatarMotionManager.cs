using System;
using TPFive.Game.Avatar.Timeline;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Motion
{
    /// <summary>
    /// This interface is the business logic of avatar motion.
    /// </summary>
    public interface IAvatarMotionManager : IAvatarTimelineManager
    {
        // Get all motions.
        IMotionItem[] Motions { get; }

        // The count of motions.
        int MotionCount { get; }

        // The current motion Guid.
        Guid CurrentMotionUid { get; }

        // Play motion by Guid.
        void Play(Guid uid, bool loop = false);

        // Get timeline asset by Guid.
        TimelineAsset GetMotion(Guid uid);
    }
}