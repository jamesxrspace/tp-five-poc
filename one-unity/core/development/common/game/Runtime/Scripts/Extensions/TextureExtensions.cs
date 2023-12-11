using System;
using UnityEngine;

namespace TPFive.Game.Extensions
{
    /// <summary>
    /// TextureExtensions for easy to get some property of texture.
    /// </summary>
    public static class TextureExtensions
    {
        /// <summary>
        /// Get the size of a texture immediately.
        /// </summary>
        /// <param name="texture">This texture.</param>
        /// <returns>Return Vector2.</returns>
        public static Vector2 Size(this Texture texture)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            return new Vector2(texture.width, texture.height);
        }
    }
}