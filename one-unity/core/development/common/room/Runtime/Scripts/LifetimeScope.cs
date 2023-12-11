using Fusion;
using MessagePipe;
using TPFive.SCG.ServiceEco.Abstractions;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using LoginService = TPFive.Game.Login.Service;

namespace TPFive.Room
{
    [RegisterService(ServiceList = @"
TPFive.Game.UI.Service,
TPFive.Game.Mocap.Service")]
    public partial class LifetimeScope : VContainer.Unity.LifetimeScope
    {
        [SerializeField]
        private RoomSetting roomSetting;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(roomSetting);
            builder.Register<NetSceneLoader>(Lifetime.Singleton).As<INetSceneLoader, INetworkSceneManager>();
            builder.Register<NetworkObjectPool>(Lifetime.Scoped).As<INetworkObjectPool>();
            builder.Register<PlayerSystem>(Lifetime.Scoped).As<IPlayerSystem>();
            builder.Register<LoginService>(Lifetime.Scoped);
            builder.Register<RoomUserRegistrar>(Lifetime.Scoped).As<IRoomUserRegistrar>();

            // Register MessagePipe broker(s)
            var options = builder.RegisterMessagePipe();
            RegisterInstallers(builder, options);
            builder.RegisterMessageBroker<EntityData<NetPlayerData>>(options);

#if UNITY_SERVER
            builder.RegisterEntryPoint<UnityMultiplayRoomManager<FusionRoom>>().As<IRoomManager>();
#else
            builder.RegisterEntryPoint<GeneralRoomManager<FusionRoom>>().As<IRoomManager>();
            builder.RegisterEntryPoint<RoomChannelManager>().As<IChannelManager>();
#endif
        }

        partial void RegisterInstallers(IContainerBuilder builder, MessagePipeOptions options);
    }
}
