using System.Collections.Generic;
using System.Linq;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class SliderItem
    {
        public SliderItem(float minimum, float maximum, int valueCount)
        {
            Minimum = minimum;
            Maximum = maximum;
            ValueCount = valueCount;
            ValueMapping = null;
        }

        public SliderItem(Dictionary<float, float> valueMapping)
        {
            Minimum = valueMapping.Keys.Min();
            Maximum = valueMapping.Keys.Max();
            ValueCount = valueMapping.Count;
            ValueMapping = valueMapping;
        }

        public float Minimum { get; }

        public float Maximum { get; }

        public int ValueCount { get; }

        /// <summary>
        /// Gets the dictionary that key is setting value, value is actual value.
        /// </summary>
        public Dictionary<float, float> ValueMapping { get; }
    }
}
