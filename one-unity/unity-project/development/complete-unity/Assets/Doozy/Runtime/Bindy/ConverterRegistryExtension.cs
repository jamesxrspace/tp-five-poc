// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

// ReSharper disable RedundantNameQualifier
namespace Doozy.Runtime.Bindy
{
    public static partial class ConverterRegistry
    {
        [Doozy.Runtime.Common.Attributes.ExecuteOnReload]
        private static void OnReload()
        {
            s_initialized = false;
        }

        static ConverterRegistry()
        {
            Initialize();
        }
        
        /// <summary>
        /// Initializes the converter registry.
        /// </summary>
        public static void Initialize()
        {
            if (s_initialized) return;
            s_initialized = true;

            // Load all converters that are defined in the project
            // This operation loads all converters defined in the project and adds them to the registry
            // This is the recommended way to load the converters as it is and Ahead of Time (AOT) operation
            LoadBuiltInConverters();
        }            
    
        private static void LoadBuiltInConverters()
        {
            AddConverter(new Doozy.Runtime.Bindy.Converters.BoolToFloatConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.BoolToIntConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.BoolToStringConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.ColorToStringConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.DateTimeToStringConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.EnumToStringConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.FloatToBoolConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.FloatToStringConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.IntToBoolConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.IntToStringConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.SpriteToTexture2DConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.StringToBoolConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.StringToColorConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.StringToDateTimeConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.StringToFloatConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.StringToIntConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.StringToVector2Converter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.StringToVector3Converter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.StringToVector4Converter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.Texture2DtoSpriteConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.Vector2ToStringConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.Vector3ToStringConverter());
            AddConverter(new Doozy.Runtime.Bindy.Converters.Vector4ToStringConverter());
        }
    }
}

