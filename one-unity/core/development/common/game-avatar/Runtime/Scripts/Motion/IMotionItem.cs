using System;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Motion
{
    public interface IMotionItem
    {
        Guid Uid { get; }

        TimelineAsset Asset { get; }
    }
}