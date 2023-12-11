using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using VContainer;

namespace TPFive.Game.Logging
{
    using TPFive.SCG.Bridge.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;

    using CrossBridge = TPFive.Cross.Bridge;

    [Dispose]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private readonly Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;

        private readonly Dictionary<System.Type, ILogger> _cachedLogger = new ();

        [Inject]
        public Service(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            CrossBridge.Logging = Logging;
        }

        /// <summary>
        /// This logging method is primarily used from Visual Scripting nodes. As
        /// visual scripting can not use DI for logger creation, this method will
        /// serve this purpose.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        [DelegateFrom(DelegateName = "Logging")]
        public void Logging(System.Type t, int level, object message)
        {
            var result = _cachedLogger.TryGetValue(t, out var logger);
            if (!result)
            {
                logger = _loggerFactory.CreateLogger(t);
                _cachedLogger.Add(t, logger);
            }

            switch (level)
            {
                case (int)Cross.LoggingLevel.EditorDebug:
                    logger.LogEditorDebug(message.ToString());
                    break;
                case (int)Cross.LoggingLevel.Debug:
                    logger.LogDebug(message.ToString());
                    break;
                case (int)Cross.LoggingLevel.Info:
                    logger.LogInformation(message.ToString());
                    break;
                case (int)Cross.LoggingLevel.Warning:
                    logger.LogWarning(message.ToString());
                    break;
                case (int)Cross.LoggingLevel.Error:
                    logger.LogError(message.ToString());
                    break;
                case (int)Cross.LoggingLevel.Critical:
                    logger.LogCritical(message.ToString());
                    break;
            }
        }

        private void HandleDispose(bool disposing)
        {
            if (disposing)
            {
                _cachedLogger.Clear();
            }
        }
    }
}