using System.Reflection;
using UnityEngine;
using UnityEngine.Timeline;

namespace TPFive.Creator.MotionConvertTool
{
    /// <summary>
    /// Copy <see cref="AudioPlayableAsset"/> from source clip to target clip
    /// </summary>
    public class AudioPlayableCopy : PlayableCopy
    {
        public override void Copy(TimelineClip sourceClip, TimelineClip targetClip, TrackData trackData)
        {
            base.Copy(sourceClip, targetClip, trackData);

            if (sourceClip.asset is not AudioPlayableAsset sourceAsset)
            {
                Debug.LogError("sourceAsset is not AudioPlayableAsset");
                return;
            }

            if (targetClip.asset is not AudioPlayableAsset targetAsset)
            {
                Debug.LogError("targetAsset is not AudioPlayableAsset");
                return;
            }

            targetAsset.clip = sourceAsset.clip;
            targetAsset.loop = sourceAsset.loop;

            var clipField = typeof(AudioPlayableAsset).GetField("m_ClipProperties", BindingFlags.NonPublic | BindingFlags.Instance);
            var volume = clipField?.GetValue(sourceAsset);
            clipField?.SetValue(targetAsset, volume);
        }
    }
}