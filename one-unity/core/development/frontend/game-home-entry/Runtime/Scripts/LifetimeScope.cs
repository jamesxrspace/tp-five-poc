using MessagePipe;
using TPFive.Game.Home.Entry;
using TPFive.Game.Mocap;
using TPFive.SCG.ServiceEco.Abstractions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Home.Entry
{
    [RegisterService(ServiceList = @"
        TPFive.Game.Space.Service,
        TPFive.Game.UI.Service,
        TPFive.Game.Mocap.Service")]
    public sealed partial class LifetimeScope : VContainer.Unity.LifetimeScope
    {
        [SerializeField]
        private MocapServiceSettings mocapServiceSettings;

        [SerializeField]
        private ReelEntryTemplateSetting reelEntryTemplateSetting;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe(pipeOptions => { });
            RegisterInstallers(builder, options);
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
            builder.RegisterInstance(mocapServiceSettings);
            builder.RegisterInstance(reelEntryTemplateSetting);
            builder.RegisterEntryPoint<Bootstrap>();
        }

        partial void RegisterInstallers(IContainerBuilder builder, MessagePipeOptions options);
    }
}