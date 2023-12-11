using UnityEngine;

namespace TPFive.Game.Utils
{
    public static class EulerAnglesUtility
    {
        /// <summary>
        /// Normalize the degree to [0, 360).
        /// </summary>
        /// <param name="degree">degree.</param>
        /// <returns>normalized degree.</returns>
        public static float GetNormalizeDegree(float degree)
        {
            float normalize = degree % 360f;
            if (normalize < 0)
            {
                normalize += 360f;
            }

            return normalize;
        }

        /// <summary>
        /// Normalize the degree of euler angles to [0, 360).
        /// </summary>
        /// <param name="eulerAngles">euler angles.</param>
        /// <returns>normalized euler angles.</returns>
        public static Vector3 GetNormalized(Vector3 eulerAngles)
        {
            eulerAngles.x = GetNormalizeDegree(eulerAngles.x);
            eulerAngles.y = GetNormalizeDegree(eulerAngles.y);
            eulerAngles.z = GetNormalizeDegree(eulerAngles.z);

            return eulerAngles;
        }

        /// <summary>
        /// Whether the current degree is in the range of min value and max value.
        /// </summary>
        /// <param name="currentDegree">the degree you want to check.</param>
        /// <param name="minDegree">minimum degree of the range.</param>
        /// <param name="maxDegree">maximum degree of the range.</param>
        /// <returns>If TRUE means in range, otherwise not.</returns>
        public static bool IsInRange(float currentDegree, float minDegree, float maxDegree)
        {
            // make sure all of degrees is in domain [0, 360).
            var normalizeVal = GetNormalizeDegree(currentDegree);
            var normalizeMin = GetNormalizeDegree(minDegree);
            var normalizeMax = GetNormalizeDegree(maxDegree);

            if (normalizeMin < normalizeMax)
            {
                return MathUtility.IsInRange(normalizeVal, normalizeMin, normalizeMax);
            }

            // maybe the min degree is greater than the max degree.
            // ex: min:315, max: 45
            // check the value not in range [46, 314] means in range [315, 45].
            return !MathUtility.IsInRange(normalizeVal, normalizeMax + 1, normalizeMin - 1);
        }
    }
}
