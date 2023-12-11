using System;
using TPFive.Game.Extensions;
using UnityEngine;

namespace TPFive.Game.Utils
{
    /// <summary>
    /// ViewportUtility is a helper for calculate viewport ratio or uv base on two view.
    /// </summary>
    public static class ViewportUtility
    {
        private static readonly Vector3[] WorldCorners = new Vector3[4];

        /// <summary>
        /// Transform the from input size to target size with aspect ratio.
        /// This will fit the target size.
        ///
        /// input(9:16)                    target(16:9)
        ///  ______                   ___________________
        /// |######|                 |       ######      |
        /// |######|                 |       ######      |
        /// |######|      ------>    |       ######      |
        /// |######|                 |       ######      |
        /// |######|                 |       ######      |
        /// |######|                 |_______######______|
        /// input(16:9)                    target(16:9)
        ///                           ___________________
        ///                          |###################|
        ///  ______                  |###################|
        /// |######|      ------>    |###################|
        /// |######|                 |###################|
        ///                          |###################|
        ///                          |###################|
        ///
        /// input(16:3)                    target(16:9)
        ///                           ___________________
        ///                          |                   |
        ///  ______                  |                   |
        /// |######|      ------>    |###################|
        ///                          |###################|
        ///                          |                   |
        ///                          |___________________|
        /// .
        /// </summary>
        public static Vector2 TransformViewport(Vector2 currentSize, Vector2 targetSize)
        {
            if (currentSize.x == 0
                || currentSize.y == 0
                || targetSize.x == 0
                || targetSize.y == 0)
            {
                throw new Exception($"TransformViewport fail : There are zero value in currentSize={currentSize} or targetSize={targetSize}.");
            }

            var currentAspectRatio = currentSize.GetRatio();
            var targetAspectRatio = targetSize.GetRatio();

            // Fit target W/H
            var size = targetSize;

            if (currentAspectRatio > targetAspectRatio)
            {
                // Fit target width
                size = currentSize * (targetSize.x / currentSize.x);
            }
            else if (currentAspectRatio < targetAspectRatio)
            {
                // Fit target height
                size = currentSize * (targetSize.y / currentSize.y);
            }

            return size;
        }

        /// <summary>
        /// TransformViewportToUV is calculate input size to fit the target size ratio.
        /// </summary>
        /// <returns>Vector2[0] = tiling, Vector2[1]= offset.</returns>
        public static Vector2[] TransformViewportToUV(Vector2 inputSize, Vector2 targetSize)
        {
            var tiling = Vector2.one;
            var offset = Vector2.zero;

            if (inputSize.x == 0
                || inputSize.y == 0
                || targetSize.x == 0
                || targetSize.y == 0)
            {
                throw new Exception($"{nameof(TransformViewportToUV)} fail : There are zero value inputSize={inputSize}, targetSize={targetSize}.");
            }

            var contentRatio = inputSize.GetRatio();
            var screenRatio = targetSize.GetRatio();

            if (contentRatio > screenRatio)
            {
                // Fit target width
                tiling.y = contentRatio / screenRatio;
                offset.y = (tiling.y - 1) / -2;
            }
            else if (contentRatio < screenRatio)
            {
                // Fit target height
                tiling.x = screenRatio / contentRatio;
                offset.x = (tiling.x - 1) / -2;
            }// The else case is fit target W/H is (1,1,0,0)

            return new Vector2[] { tiling, offset };
        }

        /// <summary>
        /// Convert RectTransform rect to screen space.
        /// </summary>
        public static Bounds GetRectTransformBounds(RectTransform transform)
        {
            transform.GetWorldCorners(WorldCorners);
            var bounds = new Bounds(WorldCorners[0], Vector3.zero);
            for (int i = 1; i < 4; ++i)
            {
                bounds.Encapsulate(WorldCorners[i]);
            }

            return bounds;
        }
    }
}