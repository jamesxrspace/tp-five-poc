using System;
using UnityEditor;
using UnityEngine;

namespace TPFive.Game.Editor
{
    /// <summary>
    /// Configurate the settings of log type for play mode.
    /// </summary>
    [FilePath("Assets/Editor/Logging/PlayModeLogConfig.asset", FilePathAttribute.Location.ProjectFolder)]
    [CreateAssetMenu(fileName = "PlayModeLogConfig.asset", menuName = "TPFive/Logging/PlayModeLogConfig")]
    internal partial class PlayModeLogConfig : ScriptableSingleton<PlayModeLogConfig>
    {
        [SerializeField]
        private LogSetting[] logSettings;

        private static LogSetting[] GetLogSettings()
        {
            if (instance != null && instance.logSettings != null)
            {
                return instance.logSettings;
            }

            return Array.Empty<LogSetting>();
        }

#pragma warning disable IDE0051 // Remove unused private members
        [InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode()
        {
            var settings = GetLogSettings();
            if (settings.Length == 0)
            {
                return;
            }

            var originalSettings = new LogSetting[settings.Length];
            for (int i = 0; i < settings.Length; i++)
            {
                var stacktraceLogType = PlayerSettings.GetStackTraceLogType(settings[i].Type);
                originalSettings[i] = new LogSetting
                {
                    Type = settings[i].Type,
                    StackTraceType = stacktraceLogType,
                };
                PlayerSettings.SetStackTraceLogType(settings[i].Type, settings[i].StackTraceType);
            }

            Application.quitting += () =>
            {
                foreach (var setting in originalSettings)
                {
                    PlayerSettings.SetStackTraceLogType(setting.Type, setting.StackTraceType);
                }
            };
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
