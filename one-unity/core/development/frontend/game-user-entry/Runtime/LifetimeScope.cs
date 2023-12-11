using TPFive.Game.Messages;

namespace TPFive.Game.User.Entry
{
    using MessagePipe;
    using TPFive.SCG.ServiceEco.Abstractions;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    [RegisterService(ServiceList = @"
TPFive.Game.User.Service,
TPFive.Game.RealtimeChat.Service")]
    public sealed partial class LifetimeScope : VContainer.Unity.LifetimeScope
    {
        [SerializeField]
        private RealtimeChat.AgoraRtcConfig agoraRtcConfig;
        [SerializeField]
        private Settings settings;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();

            this.RegisterInstallers(builder, options);
            builder.RegisterMessageBroker<QueryEntityData<User>>(options);
            builder.RegisterInstance(this.settings);
            builder.RegisterInstance(this.agoraRtcConfig);

            builder.RegisterEntryPoint<Bootstrap>();
        }

        partial void RegisterInstallers(IContainerBuilder builder, MessagePipeOptions options);
    }
}
