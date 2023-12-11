using System.Collections.Generic;
using System.Linq;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.App.Entry;
using TPFive.Game.SceneFlow;
using TPFive.Room;
using LifetimeScope = TPFive.Home.Entry.LifetimeScope;

namespace TPFive.Game.Home.Entry
{
    public class SpaceListViewModel : ViewModelBase
    {
        private readonly IPublisher<ChangeScene> pubSceneLoading;
        private readonly IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel;
        private readonly Settings appEntrySettings;
        private readonly LifetimeScope _lifetimeScope;
        private readonly IOpenRoomCmd openRoomCmd;
        private readonly ILoggerFactory loggerFactory;
        private bool disposed;
        private SimpleCommand showCommand;
        private SimpleCommand hideCommand;
        private InteractionRequest loadTextureRequest;
        private InteractionRequest releaseTextureRequest;
        private ObservableList<SpaceViewModel> items = new ObservableList<SpaceViewModel>();

        public SpaceListViewModel(
            IPublisher<ChangeScene> pubSceneLoading,
            IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope,
            IOpenRoomCmd openRoomCmd,
            ILoggerFactory loggerFactory)
        {
            showCommand = new SimpleCommand(OnShowCommand);
            hideCommand = new SimpleCommand(OnHideCommand);
            loadTextureRequest = new InteractionRequest();
            releaseTextureRequest = new InteractionRequest();
            this.pubSceneLoading = pubSceneLoading;
            this.pubLoadContentLevel = pubLoadContentLevel;
            this.appEntrySettings = appEntrySettings;
            this._lifetimeScope = lifetimeScope;
            this.openRoomCmd = openRoomCmd;
            this.loggerFactory = loggerFactory;
        }

        ~SpaceListViewModel()
        {
            Dispose(false);
        }

        public ObservableList<SpaceViewModel> Items => items;

        public IInteractionRequest LoadTextureRequest => loadTextureRequest;

        public ICommand ShowCommand => showCommand;

        public ICommand HideCommand => hideCommand;

        public IInteractionRequest ReleaseTextureRequest => releaseTextureRequest;

        public void SetItems(List<Space.Space> spaces)
        {
            ClearItems();

            var spaceViewModels = spaces.Select(space => new SpaceViewModel(
                space,
                pubSceneLoading,
                pubLoadContentLevel,
                appEntrySettings,
                _lifetimeScope,
                openRoomCmd,
                loggerFactory)).ToList();

            Items.AddRange(spaceViewModels);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                ClearItems();
            }

            base.Dispose(disposing);

            disposed = true;
        }

        private void OnHideCommand()
        {
            releaseTextureRequest.Raise();

            ClearItems();
        }

        private void OnShowCommand()
        {
            loadTextureRequest.Raise();
        }

        private void ClearItems()
        {
            foreach (var item in items)
            {
                item?.Dispose();
            }

            Items.Clear();
        }
    }
}
