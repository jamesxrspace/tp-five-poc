using MessagePipe;
using QFSW.QC;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.Assist.Entry
{
    using QFSW.QC;
    using TPFive.SCG.ServiceEco.Abstractions;

    using GameMessages = TPFive.Game.Messages;

    [RegisterService(ServiceList = @"
TPFive.Game.Assist.Service")]
    public sealed partial class LifetimeScope : VContainer.Unity.LifetimeScope
    {
        public Settings settings;
        public SceneSettings sceneSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe(pipeOptions => { });

            RegisterMessageUseDependencies(builder, options);
            RegisterInstallers(builder, options);

            builder.RegisterInstance(settings);
            builder.RegisterInstance(sceneSettings);

            builder.RegisterEntryPoint<Bootstrap>();
        }

        private void RegisterMessageUseDependencies(IContainerBuilder builder, MessagePipeOptions options)
        {
            builder.RegisterMessageBroker<GameMessages.AssistMode>(options);
        }

        partial void RegisterInstallers(IContainerBuilder builder, MessagePipeOptions options);

        [System.Serializable]
        public class SceneSettings
        {
            public QuantumConsole quantumConsole;
            public GameObject buttonGameObject;
        }
    }
}
