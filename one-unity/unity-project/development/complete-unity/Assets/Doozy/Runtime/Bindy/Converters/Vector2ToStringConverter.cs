﻿// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Bindy.Converters
{
    /// <summary>
    /// Converts a Vector2 value to a string value and vice-versa.
    /// This converter takes a Vector2 value and converts it to a string value by concatenating its x and y components separated by a comma.
    /// It also performs the reverse conversion by parsing the string and returning a Vector2 value with the parsed components.
    /// </summary>
    public class Vector2ToStringConverter : IValueConverter
    {
        /// <summary>
        /// Flag that determines whether the converter should be registered to the converter registry refreshing the list of available converters.
        /// This is useful for special converters that are not registered to the converter registry by default.
        /// </summary>
        public bool registerToConverterRegistry => true;
        
        /// <summary>
        /// The source type to convert from.
        /// </summary>
        public Type sourceType => typeof(Vector2);

        /// <summary>
        /// The target type to convert to.
        /// </summary>
        public Type targetType => typeof(string);

        /// <summary>
        /// Determines whether the converter can convert between the specified source and target types.
        /// </summary>
        /// <param name="source">The source type to convert from.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>True if the conversion is supported, otherwise false.</returns>
        public bool CanConvert(Type source, Type target) =>
            source == typeof(Vector2) && target == typeof(string);

        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type target)
        {
            if (value == null)
                return null;

            if (target != typeof(string))
                throw new ArgumentException($"Invalid target type: {target}. Expected: {typeof(string)}.");

            if (value is Vector2 vectorValue)
                return $"{vectorValue.x},{vectorValue.y}";

            throw new ArgumentException($"Invalid source type: {value.GetType()}. Expected: {typeof(Vector2)}.");
        }
    }
}
