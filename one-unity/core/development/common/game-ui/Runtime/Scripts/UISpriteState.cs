using System;
using UnityEngine;

namespace TPFive.Game.UI
{
    [Serializable]
    public struct UISpriteState : IEquatable<UISpriteState>
    {
        [SerializeField]
        private Sprite highlightedSprite;

        [SerializeField]
        private Sprite pressedSprite;

        [SerializeField]
        private Sprite selectedSprite;

        [SerializeField]
        private Sprite disabledSprite;

        /// <summary>
        /// Gets or sets highlighted sprite.
        /// </summary>
        /// <value>
        /// Highlighted sprite.
        /// </value>
        public Sprite HighlightedSprite
        {
            get { return highlightedSprite; } set { highlightedSprite = value; }
        }

        /// <summary>
        /// Gets or sets pressed sprite.
        /// </summary>
        /// <value>
        /// Pressed sprite.
        /// </value>
        public Sprite PressedSprite
        {
            get { return pressedSprite; } set { pressedSprite = value; }
        }

        /// <summary>
        /// Gets or sets selected sprite.
        /// </summary>
        /// <value>
        /// Selected sprite.
        /// </value>
        public Sprite SelectedSprite
        {
            get { return selectedSprite; } set { selectedSprite = value; }
        }

        /// <summary>
        /// Gets or sets disabled sprite.
        /// </summary>
        /// <value>
        /// Disabled sprite.
        /// </value>
        public Sprite DisabledSprite
        {
            get { return disabledSprite; } set { disabledSprite = value; }
        }

        public static bool operator ==(UISpriteState point1, UISpriteState point2)
        {
            return point1.Equals(point2);
        }

        public static bool operator !=(UISpriteState point1, UISpriteState point2)
        {
            return !point1.Equals(point2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UISpriteState))
            {
                return false;
            }

            return Equals((UISpriteState)obj);
        }

        public bool Equals(UISpriteState other)
        {
            return HighlightedSprite == other.HighlightedSprite &&
                   PressedSprite == other.PressedSprite &&
                   SelectedSprite == other.SelectedSprite &&
                   DisabledSprite == other.DisabledSprite;
        }
    }
}
