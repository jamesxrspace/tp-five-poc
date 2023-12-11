using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace TPFive.Game.Logging
{
    public static class Utility
    {
        public static ILogger CreateLogger<T>(
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory?.CreateLogger<T>() ?? NullLogger<T>.Instance;

            return logger;
        }
    }
}
