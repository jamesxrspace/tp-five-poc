using UnityEngine;

namespace TPFive.Game.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="UnityEngine.RectTransform"/>.
    /// </summary>
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Gets width of rect transform.
        /// </summary>
        /// <param name="rectTransform">The rect transform.</param>
        /// <returns>
        /// Width of rect transform.
        /// </returns>
        public static float GetWidth(this RectTransform rectTransform)
        {
            return rectTransform.rect.width;
        }

        /// <summary>
        /// Gets height of rect transform.
        /// </summary>
        /// <param name="rectTransform">The rect transform.</param>
        /// <returns>
        /// Height of rect transform.
        /// </returns>
        public static float GetHeight(this RectTransform rectTransform)
        {
            return rectTransform.rect.height;
        }

        /// <summary>
        /// Gets size of rect transform.
        /// </summary>
        /// <param name="rectTransform">The rect transform.</param>
        /// <returns>Size of rect transform.</returns>
        public static Vector2 GetSize(this RectTransform rectTransform)
        {
            return rectTransform.rect.size;
        }

        /// <summary>
        /// Sets width of rect transform.
        /// </summary>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="newWidth">The new width value.</param>
        public static void SetWidth(this RectTransform rectTransform, float newWidth)
        {
            SetSize(rectTransform, new Vector2(newWidth, rectTransform.rect.size.y));
        }

        /// <summary>
        /// Sets height of rect transform.
        /// </summary>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="newHeight">The new height value.</param>
        public static void SetHeight(this RectTransform rectTransform, float newHeight)
        {
            SetSize(rectTransform, new Vector2(rectTransform.rect.size.x, newHeight));
        }

        /// <summary>
        /// Sets size of rect transform.
        /// </summary>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="newSize">The new size value.</param>
        public static void SetSize(this RectTransform rectTransform, Vector2 newSize)
        {
            var oldSize = rectTransform.rect.size;
            var deltaSize = newSize - oldSize;
            var pivot = rectTransform.pivot;

            rectTransform.offsetMin =
                rectTransform.offsetMin - new Vector2(deltaSize.x * pivot.x, deltaSize.y * pivot.y);
            rectTransform.offsetMax =
                rectTransform.offsetMax + new Vector2(deltaSize.x * (1f - pivot.x), deltaSize.y * (1f - pivot.y));
        }

        /// <summary>
        /// Sets <see cref="UnityEngine.RectTransform.anchorMin"/> and <see cref="UnityEngine.RectTransform.anchorMax"/> to the specified value.
        /// </summary>
        /// <param name="rectTransform">rect transform.</param>
        /// <param name="value">specified value.</param>
        public static void SetAnchors(this RectTransform rectTransform, Vector2 value)
        {
            rectTransform.anchorMin = value;
            rectTransform.anchorMax = value;
        }

        /// <summary>
        /// Sets <see cref="UnityEngine.RectTransform.pivot"/>, <see cref="UnityEngine.RectTransform.anchorMin"/>
        /// and <see cref="UnityEngine.RectTransform.anchorMax"/> to the specified value.
        /// </summary>
        /// <param name="rectTransform">rect transform.</param>
        /// <param name="value">specified value.</param>
        public static void SetPivotAndAnchors(this RectTransform rectTransform, Vector2 value)
        {
            rectTransform.pivot = value;
            rectTransform.anchorMin = value;
            rectTransform.anchorMax = value;
        }
    }
}
