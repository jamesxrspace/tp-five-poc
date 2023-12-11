using MessagePipe;
using TPFive.Creator.Components.Interactable;
using UnityEngine;
#if HAS_HIGH_PRIORITY_ENTRY
#else
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Unity3D;
#endif
using VContainer;
using VContainer.Unity;

namespace TPFive.Creator.Entry
{
    using CreatorMessages = TPFive.Creator.Messages;
    using GameMessages = TPFive.Game.Messages;

#if HAS_HIGH_PRIORITY_ENTRY
#else
    using GameConfig = TPFive.Game.Config;
    using GameResource = TPFive.Game.Resource;
    using GameSceneFlow = TPFive.Game.SceneFlow;

    using GameObjectPool = TPFive.Game.ObjectPool;

    using GameAudio = TPFive.Game.Audio;
    using GameHud = TPFive.Game.Hud;
    using GameLocalization = TPFive.Game.Localization;

    using GameActor = TPFive.Game.Actor;

    using GameReferenceLocator = TPFive.Game.ReferenceLocator;
#endif

    using CreatorMessageRepo = TPFive.Creator.MessageRepo;

    public sealed partial class LifetimeScope : VContainer.Unity.LifetimeScope
    {
        [SerializeField]
        private Settings settings;

        [SerializeField]
        private SceneSettings sceneSettings;

#if HAS_HIGH_PRIORITY_ENTRY
#else
        private LoggerConfiguration _loggerConfiguration;
        private Serilog.Core.Logger _log;
#endif

        public Settings Settings
        {
            set { settings = value; }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe(pipeOptions => { });
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

#if HAS_HIGH_PRIORITY_ENTRY
#else
            RegisterLoggerUseDependencies(builder);
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
#endif
            RegisterMessageUseDependencies(builder, options);
            RegisterInstallers(builder, options);

            builder.RegisterInstance(settings);
            builder.RegisterInstance(sceneSettings);

            builder.RegisterEntryPoint<Bootstrap>();
        }

#if HAS_HIGH_PRIORITY_ENTRY
#else
        private void RegisterLoggerUseDependencies(IContainerBuilder builder)
        {
            var factory = new LoggerFactory();

            _loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Unity3D(outputTemplate: "[{Level:u3}][{SourceContext}] {Message:j}{NewLine}{Exception}\n");

            _loggerConfiguration.MinimumLevel.Debug();

            _loggerConfiguration
                .WriteTo.Seq("http://localhost:5341");

            _log = _loggerConfiguration.CreateLogger();

            factory.AddSerilog(_log);

            builder.RegisterInstance<ILoggerFactory>(factory);
        }
#endif

        private void RegisterMessageUseDependencies(IContainerBuilder builder, MessagePipeOptions options)
        {
#if HAS_HIGH_PRIORITY_ENTRY
#else
            builder.RegisterMessageBroker<GameMessages.BootstrapJustStarted>(options);
            builder.RegisterMessageBroker<GameMessages.BootstrapSetupDone>(options);

            builder.RegisterMessageBroker<GameMessages.SceneLoading>(options);
            builder.RegisterMessageBroker<GameMessages.SceneLoaded>(options);
            builder.RegisterMessageBroker<GameMessages.SceneUnloading>(options);
            builder.RegisterMessageBroker<GameMessages.SceneUnloaded>(options);

            builder.RegisterMessageBroker<GameMessages.MultiPhaseSetupDone>(options);

            builder.RegisterMessageBroker<GameMessages.HudMessage>(options);
            builder.RegisterMessageBroker<Game.SceneFlow.ChangeScene>(options);
#endif
            Debug.Log("RegisterMessageUseDependencies - MarkerMessage");
            builder.RegisterMessageBroker<CreatorMessages.MarkerMessage>(options);
            builder.RegisterMessageBroker<ContentSubLevelLoaded>(options);
        }

        private void RegisterInstallers(IContainerBuilder builder, MessagePipeOptions options)
        {
#if HAS_HIGH_PRIORITY_ENTRY
#else
            {
                var installer = new GameConfig.Installer(options);
                installer.Install(builder);
            }
            {
                var installer = new GameResource.Installer(options);
                installer.Install(builder);
            }
            {
                var installer = new GameSceneFlow.Installer(options);
                installer.Install(builder);
            }
            {
                var installer = new GameObjectPool.Installer(options);
                installer.Install(builder);
            }
            {
                var installer = new GameActor.Installer(options);
                installer.Install(builder);
            }
            {
                var installer = new GameAudio.Installer(options);
                installer.Install(builder);
            }
            {
                var installer = new GameHud.Installer(options);
                installer.Install(builder);
            }
            {
                var installer = new GameLocalization.Installer(options);
                installer.Install(builder);
            }
            {
                var installer = new GameReferenceLocator.Installer(options);
                installer.Install(builder);
            }
#endif
            {
                var installer = new Game.Interactable.Toolkit.Installer(options);
                installer.Install(builder);
            }

            {
                var installer = new CreatorMessageRepo.Installer(options);
                installer.Install(builder);
            }
        }

        [System.Serializable]
        public class SceneSettings
        {
        }
    }
}
