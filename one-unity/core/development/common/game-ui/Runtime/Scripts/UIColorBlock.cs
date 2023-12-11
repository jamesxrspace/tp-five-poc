using System;
using UnityEngine;

namespace TPFive.Game.UI
{
    [Serializable]
    public struct UIColorBlock : IEquatable<UIColorBlock>
    {
        [SerializeField]
        private Color normalColor;
        [SerializeField]
        private Color highlightedColor;
        [SerializeField]
        private Color pressedColor;
        [SerializeField]
        private Color selectedColor;
        [SerializeField]
        private Color disabledColor;
        [Range(1, 5)]
        [SerializeField]
        private float colorMultiplier;
        [SerializeField]
        private float fadeDuration;

        public static UIColorBlock DefaultColorBlock
        {
            get
            {
                var c = new UIColorBlock
                {
                    normalColor = new Color32(255, 255, 255, 255),
                    highlightedColor = new Color32(245, 245, 245, 255),
                    pressedColor = new Color32(200, 200, 200, 255),
                    selectedColor = new Color32(245, 245, 245, 255),
                    disabledColor = new Color32(200, 200, 200, 128),
                    colorMultiplier = 1.0f,
                    fadeDuration = 0.1f,
                };
                return c;
            }
        }

        public Color NormalColor
        {
            get { return normalColor; } set { normalColor = value; }
        }

        public Color HighlightedColor
        {
            get { return highlightedColor; } set { highlightedColor = value; }
        }

        public Color PressedColor
        {
            get { return pressedColor; } set { pressedColor = value; }
        }

        public Color SelectedColor
        {
            get { return selectedColor; } set { selectedColor = value; }
        }

        public Color DisabledColor
        {
            get { return disabledColor; } set { disabledColor = value; }
        }

        public float ColorMultiplier
        {
            get { return colorMultiplier; } set { colorMultiplier = value; }
        }

        public float FadeDuration
        {
            get { return fadeDuration; } set { fadeDuration = value; }
        }

        public static bool operator ==(UIColorBlock point1, UIColorBlock point2)
        {
            return point1.Equals(point2);
        }

        public static bool operator !=(UIColorBlock point1, UIColorBlock point2)
        {
            return !point1.Equals(point2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UIColorBlock))
            {
                return false;
            }

            return Equals((UIColorBlock)obj);
        }

        public bool Equals(UIColorBlock other)
        {
            return NormalColor == other.NormalColor &&
                HighlightedColor == other.HighlightedColor &&
                PressedColor == other.PressedColor &&
                SelectedColor == other.SelectedColor &&
                DisabledColor == other.DisabledColor &&
                ColorMultiplier == other.ColorMultiplier &&
                FadeDuration == other.FadeDuration;
        }
    }
}
