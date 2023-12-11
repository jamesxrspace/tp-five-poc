namespace TPFive.Game.Utils
{
    public static class MathUtility
    {
        /// <summary>
        /// Whether the current value is in the range of min value and max value.
        /// </summary>
        /// <param name="curtValue">current value.</param>
        /// <param name="minValue">min value.</param>
        /// <param name="maxValue">max value.</param>
        /// <returns>If TRUE means in range, otherwise not.</returns>
        public static bool IsInRange(float curtValue, float minValue, float maxValue)
        {
            return minValue <= curtValue && curtValue <= maxValue;
        }
    }
}