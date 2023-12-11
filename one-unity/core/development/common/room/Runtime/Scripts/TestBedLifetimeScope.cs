using Microsoft.Extensions.Logging;
using VContainer;

namespace TPFive.Room
{
    public class TestBedLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(new LoggerFactory()).As<ILoggerFactory>();
            base.Configure(builder);
        }
    }
}
