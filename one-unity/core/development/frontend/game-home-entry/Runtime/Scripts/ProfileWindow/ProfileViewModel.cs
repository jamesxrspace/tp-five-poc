using System;
using System.Linq;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using MessagePipe;
using TPFive.Game.App.Entry;
using LifetimeScope = TPFive.Home.Entry.LifetimeScope;

namespace TPFive.Game.Home.Entry
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly SimpleCommand closeCommand;
        private readonly SimpleCommand editAvatarCommand;
        private readonly SimpleCommand mocapCommand;
        private readonly InteractionRequest closeRequest;
        private readonly IPublisher<SceneFlow.ChangeScene> pubSceneLoading;
        private readonly Settings appEntrySettings;
        private readonly LifetimeScope lifetimeScope;
        private readonly Game.Mocap.IService mocapService;

        public ProfileViewModel(
            IPublisher<SceneFlow.ChangeScene> pubSceneLoading,
            Mocap.IService mocapService,
            Settings appEntrySettings,
            LifetimeScope homeLifetimeScope)
        {
            closeCommand = new SimpleCommand(OnCloseCommand);
            editAvatarCommand = new SimpleCommand(OnEditAvatarCommand);
            mocapCommand = new SimpleCommand(OnMocapCommand);

            closeRequest = new InteractionRequest();

            this.mocapService = mocapService;
            this.appEntrySettings = appEntrySettings;
            this.pubSceneLoading = pubSceneLoading;
            this.lifetimeScope = homeLifetimeScope;
        }

        public ICommand CloseCommand => closeCommand;

        public ICommand EditAvatarCommand => editAvatarCommand;

        public ICommand MocapCommand => mocapCommand;

        public IInteractionRequest CloseRequest => closeRequest;

        private void OnEditAvatarCommand()
        {
            GotoAvatarEditEntry();
        }

        private void OnMocapCommand()
        {
            if (mocapService.IsMocapEnabled)
            {
                mocapService.DisableMocap();
            }
            else
            {
                var options = Mocap.CaptureOptions.None;
                options.EnableUpperBody();
                options.EnableFace();
                mocapService.EnableMocap(options);
            }
        }

        private void OnCloseCommand()
        {
            closeRequest.Raise();
        }

        private void GotoAvatarEditEntry()
        {
            var fromEntryProperty = GetSceneProperty("HomeEntry");
            var toEntryProperty = GetSceneProperty("AvatarEditEntry");

            pubSceneLoading.Publish(new Game.SceneFlow.ChangeScene()
            {
                FromCategory = fromEntryProperty.category,
                FromTitle = fromEntryProperty.addressableKey,
                FromCategoryOrder = fromEntryProperty.categoryOrder,
                FromSubOrder = fromEntryProperty.subOrder,
                ToCategory = toEntryProperty.category,
                ToTitle = toEntryProperty.addressableKey,
                ToCategoryOrder = toEntryProperty.categoryOrder,
                ToSubOrder = toEntryProperty.subOrder,
                LifetimeScope = lifetimeScope,
            });

            closeRequest.Raise();
        }

        private SceneProperty GetSceneProperty(string title)
        {
            return appEntrySettings.ScenePropertyList
                .FirstOrDefault(x => x.title.Equals(title, StringComparison.Ordinal));
        }
    }
}
