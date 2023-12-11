using System;
using UnityEngine;

namespace TPFive.Game.Editor
{
    /// <summary>
    /// Configurate the settings of log type for play mode.
    /// </summary>
    internal partial class PlayModeLogConfig
    {
        /// <summary>
        /// The settings of LogType.
        /// </summary>
        [Serializable]
        private struct LogSetting
        {
            public LogType Type;
            public StackTraceLogType StackTraceType;
        }
    }
}
