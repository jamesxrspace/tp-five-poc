using System;
using MessagePipe;
using TPFive.Game.Login.Entry.Models;
using TPFive.SCG.ServiceEco.Abstractions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.Login.Entry
{
    [RegisterService(ServiceList = @"
        TPFive.Game.UI.Service")]
    public sealed partial class LifetimeScope : VContainer.Unity.LifetimeScope
    {
        [SerializeField]
        private Settings settings;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            RegisterInstallers(builder, options);
            builder.RegisterInstance(this.settings);
            builder.Register<Repository.AccountRepository>(Lifetime.Scoped).WithParameter(Array.Empty<Account>());
            builder.RegisterEntryPoint<Bootstrap>();
        }

        partial void RegisterInstallers(IContainerBuilder builder, MessagePipeOptions options);
    }
}