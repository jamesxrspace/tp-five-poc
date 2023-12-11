using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Unity3D;
using TPFive.Game.Avatar;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Logger = Serilog.Core.Logger;

namespace TPFive.Creator.MotionConvertTool
{
    public class DemoLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private AvatarFactorySettings settings;

        private LoggerConfiguration loggerConfiguration;
        private Logger log;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterLoggerUseDependencies(builder);
            builder.RegisterComponent(settings);
            builder.Register<DefaultAvatarFactory>(Lifetime.Scoped).As<IAvatarFactory>();
        }

        private void RegisterLoggerUseDependencies(IContainerBuilder builder)
        {
            var factory = new LoggerFactory();

            loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Unity3D(outputTemplate: "[{Level:u3}][{SourceContext}] {Message:j}{NewLine}{Exception}\n");

            loggerConfiguration.MinimumLevel.Debug();

            log = loggerConfiguration.CreateLogger();

            factory.AddSerilog(log);

            builder.RegisterInstance<ILoggerFactory>(factory);
        }
    }
}