using System;
using System.Collections.Generic;
using System.Linq;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using MessagePipe;
using TPFive.Game.App.Entry;
using TPFive.Game.Record;
using LifetimeScope = TPFive.Home.Entry.LifetimeScope;

namespace TPFive.Game.Home.Entry
{
    public class ReelEntryViewModel : ViewModelBase
    {
        private readonly Config.IService configService;
        private readonly SimpleCommand closeCommand;
        private readonly SimpleCommand browseCommand;
        private readonly SimpleCommand<ReelSceneDesc> createCommand;
        private readonly InteractionRequest closeRequest;
        private readonly IPublisher<SceneFlow.ChangeScene> pubSceneLoading;
        private readonly Settings appEntrySettings;
        private readonly LifetimeScope lifetimeScope;
        private List<ReelSceneDesc> sceneTemplateList = new List<ReelSceneDesc>();

        public ReelEntryViewModel(
            Config.IService configService,
            IPublisher<SceneFlow.ChangeScene> pubSceneLoading,
            Settings appEntrySettings,
            ReelEntryTemplateSetting reelEntryTemplateSetting,
            LifetimeScope homeLifetimeScope)
        {
            closeCommand = new SimpleCommand(OnCloseCommand);
            browseCommand = new SimpleCommand(OnBrowseCommand);
            createCommand = new SimpleCommand<ReelSceneDesc>(OnCreateCommand);
            closeRequest = new InteractionRequest();

            this.configService = configService;
            this.appEntrySettings = appEntrySettings;
            this.pubSceneLoading = pubSceneLoading;
            this.lifetimeScope = homeLifetimeScope;

            SceneTemplateList = CreateReelSceneList(reelEntryTemplateSetting.ReelEntryTemplates);
        }

        public ICommand CloseCommand => closeCommand;

        public ICommand BrowseReelsCommand => browseCommand;

        public ICommand CreateReelCommand => createCommand;

        public IInteractionRequest CloseRequest => closeRequest;

        public List<ReelSceneDesc> SceneTemplateList
        {
            get => sceneTemplateList;
            set => Set(ref sceneTemplateList, value, nameof(SceneTemplateList));
        }

        private List<ReelSceneDesc> CreateReelSceneList(ReelEntryTemplate[] templates)
        {
            var list = new List<ReelSceneDesc>();
            foreach (var template in templates)
            {
                list.Add(template.ReelSceneDesc);
            }

            return list;
        }

        private void OnBrowseCommand()
        {
            var entryParam = new ReelSceneEntryParameter()
            {
                Entry = ReelSceneEntryParameter.EntryType.Browse,

                // we added a mock reel url here because ReelManager.StartViewerMode() requires a reel url.
                ReelUrl =
                    "https://d10cttm21ldbr4.cloudfront.net/test/reel/combine025d419c-d156-4e31-885a-017e71ab9686.xrs",
            };

            GotoReelScene(entryParam);
        }

        private void OnCreateCommand(ReelSceneDesc sceneDesc)
        {
            var entryParam = new ReelSceneEntryParameter()
            {
                Entry = ReelSceneEntryParameter.EntryType.Create,
                SceneDesc = sceneDesc,
            };

            GotoReelScene(entryParam);
        }

        private void OnCloseCommand()
        {
            closeRequest.Raise();
        }

        private void GotoReelScene(ReelSceneEntryParameter entryParam)
        {
            configService.SetSystemObjectValue(
                Config.Constants.RuntimeLocalProviderKind,
                nameof(ReelSceneEntryParameter),
                entryParam);

            var fromEntryProperty = GetSceneProperty("HomeEntry");
            var toEntryProperty = GetSceneProperty("RecordEntry");

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
