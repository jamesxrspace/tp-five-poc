using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace TPFive.Room
{
    public static class LoggerFactoryExtensions
    {
        public static ILogger<T> CreateLogger<T>(this ILoggerFactory factory)
        {
            if (factory == null)
            {
                return NullLogger<T>.Instance;
            }
            else
            {
                return Microsoft.Extensions.Logging.LoggerFactoryExtensions.CreateLogger<T>(factory);
            }
        }
    }
}