using MessagePipe;
using TPFive.Game.Avatar.Factory;
using TPFive.Game.Record.Entry.Settings;
using TPFive.Game.Record.Scene.SpawnPoint;
using TPFive.Game.Reel.Camera;
using TPFive.SCG.ServiceEco.Abstractions;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using IReelSceneService = TPFive.Game.Record.Scene.IService;
using ReelSceneService = TPFive.Game.Record.Scene.Service;

namespace TPFive.Game.Record.Entry
{
    [RegisterService(ServiceList = @"
TPFive.Game.Mocap.Service")]
    public partial class LifetimeScope : VContainer.Unity.LifetimeScope
    {
        [SerializeField]
        private RecordSetting recordSetting;

        [SerializeField]
        private AvatarFactorySettings avatarFactorySettings;

        [SerializeField]
        private MusicToMotionPlayerConfig musicToMotionPlayerConfig;

        [SerializeField]
        private ReelDirectorConfig reelDirectorConfig;

        [SerializeField]
        private ReelAttachmentSettings reelAttachmentSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            RegisterInstallers(builder, options);
            builder.RegisterInstance(recordSetting);
            builder.RegisterInstance(avatarFactorySettings);
            builder.RegisterInstance(musicToMotionPlayerConfig);
            builder.RegisterInstance(reelDirectorConfig);
            builder.RegisterInstance(reelAttachmentSettings);
            builder.Register<Service>(Lifetime.Scoped).As<IService>();
            builder.Register<Reel.Service>(Lifetime.Scoped).As<Reel.IService>();
            builder.Register<IReelSceneService, ReelSceneService>(Lifetime.Singleton);
            builder.RegisterComponentOnNewGameObject<ReelManager>(Lifetime.Scoped).AsSelf();
            builder.Register<MusicToMotionService>(Lifetime.Scoped);
            builder.Register<ISpawnPointService, DefaultSpawnPointService>(Lifetime.Singleton);
            builder.Register<DefaultAvatarFactory>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<ReelCameraController>(Lifetime.Scoped);
            builder.RegisterEntryPoint<Bootstrap>();
        }

        partial void RegisterInstallers(IContainerBuilder builder, MessagePipeOptions options);
    }
}
