using UnityEngine.Timeline;

namespace TPFive.Creator.MotionConvertTool
{
    /// <summary>
    /// The base class for copying a PlayableAsset to another PlayableAsset.
    /// </summary>
    public class PlayableCopy
    {
        /// <summary>
        /// Copy the base variables from source to target.
        /// </summary>
        /// <param name="sourceClip">The old project clip asset.</param>
        /// <param name="targetClip">The TP-Five clip asset.</param>
        /// <param name="trackData">All tracks in current timeline asset.</param>
        public virtual void Copy(TimelineClip sourceClip, TimelineClip targetClip, TrackData trackData)
        {
            targetClip.duration = sourceClip.duration;
            targetClip.start = sourceClip.start;
            targetClip.easeInDuration = sourceClip.easeInDuration;
            targetClip.easeOutDuration = sourceClip.easeOutDuration;
            targetClip.blendInCurveMode = sourceClip.blendInCurveMode;
            targetClip.mixInCurve = sourceClip.mixInCurve;
            targetClip.blendOutCurveMode = sourceClip.blendOutCurveMode;
            targetClip.mixOutCurve = sourceClip.mixOutCurve;
        }
    }
}