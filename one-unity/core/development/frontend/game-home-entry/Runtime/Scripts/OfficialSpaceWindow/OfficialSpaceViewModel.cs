using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.App.Entry;
using LifetimeScope = TPFive.Home.Entry.LifetimeScope;

namespace TPFive.Game.Home.Entry
{
    public class OfficialSpaceViewModel : ViewModelBase
    {
        private bool disposed;
        private SimpleCommand closeCommand;
        private InteractionRequest closeRequest;
        private SpaceGroupListViewModel spaceGroupListViewModel;

        public OfficialSpaceViewModel(
            ILoggerFactory loggerFactory,
            Space.IService spaceService,
            IPublisher<SceneFlow.ChangeScene> pubSceneLoading,
            IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope,
            Room.IOpenRoomCmd openRoomCmd)
        {
            Logger = loggerFactory.CreateLogger<OfficialSpaceViewModel>();
            spaceGroupListViewModel = new SpaceGroupListViewModel(
                loggerFactory,
                spaceService,
                pubSceneLoading,
                pubLoadContentLevel,
                appEntrySettings,
                lifetimeScope,
                openRoomCmd);
            closeCommand = new SimpleCommand(OnCloseCommand);
            closeRequest = new InteractionRequest();
        }

        public ICommand CloseCommand => closeCommand;

        public IInteractionRequest CloseRequest => closeRequest;

        public Microsoft.Extensions.Logging.ILogger Logger { get; private set; }

        public bool Disposed => disposed;

        public SpaceGroupListViewModel SpaceGroupListViewModel
        {
            get => spaceGroupListViewModel;
            set => Set(ref spaceGroupListViewModel, value, nameof(SpaceGroupListViewModel));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                SpaceGroupListViewModel?.Dispose();
            }

            disposed = true;

            base.Dispose(disposing);
        }

        private void OnCloseCommand()
        {
            closeRequest.Raise();
        }
    }
}
