using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TPFive.BuildSupport.Editor
{
    public partial class AppBuilder
    {
        public static readonly Dictionary<LogType, StackTraceLogType> LogStackTraceSettings = new()
        {
            { LogType.Log, StackTraceLogType.None },
            { LogType.Warning, StackTraceLogType.None },
        };

        public static void ConfigureLogStackTraceSettings(IDictionary<LogType, StackTraceLogType> settings)
        {
            foreach (var kvp in settings)
            {
                PlayerSettings.SetStackTraceLogType(kvp.Key, kvp.Value);
            }
        }
    }
}
