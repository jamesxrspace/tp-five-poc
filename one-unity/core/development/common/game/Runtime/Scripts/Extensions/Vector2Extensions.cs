using System;
using UnityEngine;

namespace TPFive.Game.Extensions
{
    /// <summary>
    /// Vector2Extensions for easy to calculate some result.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Inverse the Vector2 x and y.
        /// </summary>
        public static Vector2 Inverse(this Vector2 origin)
        {
            return new Vector2(origin.y, origin.x);
        }

        /// <summary>
        /// Get the ratio of Vector2 x and y.
        /// </summary>
        public static float GetRatio(this Vector2 origin)
        {
            return origin.x / origin.y;
        }
    }
}