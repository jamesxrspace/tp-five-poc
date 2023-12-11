using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace TPFive.Game.Logging
{
#nullable enable
    public static class Extensions
    {
        public static void LogEditorDebug(
            this ILogger logger,
            string? message,
            params object?[] args)
        {
#if UNITY_EDITOR
            logger.Log(LogLevel.Debug, message, args);
#endif
        }

        public static bool IsTraceEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Trace);
        }

        public static bool IsDebugEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Debug);
        }

        public static bool IsInformationEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Information);
        }

        public static bool IsWarningEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Warning);
        }

        public static bool IsErrorEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Error);
        }

        public static bool IsCriticalEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Critical);
        }
    }
#nullable disable
}
