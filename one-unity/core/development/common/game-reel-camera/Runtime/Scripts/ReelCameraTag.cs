using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TPFive.Game.Reel.Camera
{
    [CreateAssetMenu(fileName = "ReelCameraTag", menuName = "TPFive/Record/Reel Camera Tag")]
    public class ReelCameraTag : ScriptableObject
    {
        public static bool AnyIntersection(IEnumerable<ReelCameraTag> originTags, IEnumerable<ReelCameraTag> tags)
        {
            if (originTags == null || tags == null)
            {
                return false;
            }

            return originTags.Intersect(tags).Any();
        }

        public static bool AnyIntersection(IEnumerable<ReelCameraTag> originTags, ReelCameraTag tag)
        {
            if (originTags == null || tag == null)
            {
                return false;
            }

            return originTags.Contains(tag);
        }

        public static bool AllIntersection(IEnumerable<ReelCameraTag> originTags, IEnumerable<ReelCameraTag> tags)
        {
            if (originTags == null || tags == null)
            {
                return false;
            }

            return tags.All(originTags.Contains);
        }

        public bool CompareTagName(string tagName)
        {
            return string.Equals(name, tagName, StringComparison.Ordinal);
        }
    }
}
