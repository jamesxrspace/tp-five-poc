using System;

namespace TPFive.Game.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Convert enum to another enum.
        /// Use enum field name to convert.
        /// </summary>
        /// <param name="from">from which enum.</param>
        /// <param name="ignoreCase">TRUE to read enumType in case insensitive mode; FALSE to read enumType in case sensitive mode.</param>
        /// <typeparam name="T">the enum type of want to convert.</typeparam>
        /// <returns>another enum.</returns>
        public static T ConvertTo<T>(this Enum from, bool ignoreCase = true)
            where T : struct, Enum
        {
            var enumName = Enum.GetName(from.GetType(), from);

            return Enum.Parse<T>(enumName, ignoreCase);
        }

        /// <summary>
        /// Try convert enum to another enum.
        /// Use enum field name to convert.
        /// </summary>
        /// <param name="from">from which enum.</param>
        /// <param name="to">converted enum.</param>
        /// <typeparam name="T">the enum type of want to convert.</typeparam>
        /// <returns>If TRUE means convert success, otherwise not.</returns>
        public static bool TryConvertTo<T>(this Enum from, out T to)
            where T : struct, Enum
        {
            return from.TryConvertTo(true, out to);
        }

        /// <summary>
        /// Try convert enum to another enum.
        /// Use enum field name to convert.
        /// </summary>
        /// <param name="from">from which enum.</param>
        /// <param name="ignoreCase">TRUE to read enumType in case insensitive mode; FALSE to read enumType in case sensitive mode.</param>
        /// <param name="to">converted enum.</param>
        /// <typeparam name="T">the enum type of want to convert.</typeparam>
        /// <returns>If TRUE means convert success, otherwise not.</returns>
        public static bool TryConvertTo<T>(this Enum from, bool ignoreCase, out T to)
            where T : struct, Enum
        {
            var enumName = Enum.GetName(from.GetType(), from);

            return Enum.TryParse(enumName, true, out to);
        }
    }
}
