using TPFive.Game.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.AvatarEdit.Entry
{
    public sealed class LifetimeScope : VContainer.Unity.LifetimeScope
    {
        [SerializeField]
        private AvatarEditSettings editSettings;
        [SerializeField]
        private SceneSettings sceneSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(editSettings);
            builder.RegisterInstance(sceneSettings);
            builder.Register<AvatarEditController>(Lifetime.Scoped).As<IAvatarEditController>();
            builder.Register<Service>(Lifetime.Scoped).As<IService>();

            builder.RegisterEntryPoint<Bootstrap>();
        }
    }
}
