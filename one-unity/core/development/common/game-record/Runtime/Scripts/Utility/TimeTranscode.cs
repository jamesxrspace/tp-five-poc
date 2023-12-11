using System;
using System.Globalization;

namespace TPFive.Game.Record
{
    public class TimeTranscode
    {
        /// <summary>
        /// get time base float value and convert to TimeSpan format(hh:mm:ss.fff).
        /// </summary>
        /// <param name="duration">float format of seconds.</param>
        /// <returns>timeSpan format.</returns>
        public static TimeSpan Float2timespan(float duration)
        {
            return TimeSpan.FromSeconds(duration);
        }

        /// <summary>
        /// get time base float value and convert to TimeSpan format(hh:mm:ss.fff).
        /// For YOUTUBE subtitle srt file. time ONLY SUPPORT 1 digial Hour data, 1:59:59.999, not 01:59:59.999.
        /// </summary>
        /// <param name="timeString">TimeSpan format (hh:mm:ss.fff).</param>
        /// <returns>based on second float value (example: 1:01:01.123 = 3661.123).</returns>
        public static float Timespan2float(string timeString)
        {
            TimeSpan timespan_temp = TimeSpan.ParseExact(timeString, "h':'mm':'ss'.'fff", CultureInfo.InvariantCulture);
            return (float)timespan_temp.TotalSeconds;
        }
    }
}