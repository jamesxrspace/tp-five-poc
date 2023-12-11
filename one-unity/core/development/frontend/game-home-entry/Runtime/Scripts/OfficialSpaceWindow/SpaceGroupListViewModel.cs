using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.App.Entry;
using TPFive.Game.Space;
using TPFive.Home.Entry;
using TPFive.Room;
using LifetimeScope = TPFive.Home.Entry.LifetimeScope;

namespace TPFive.Game.Home.Entry
{
    public class SpaceGroupListViewModel : ViewModelBase
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IService spaceService;
        private readonly IPublisher<SceneFlow.ChangeScene> pubSceneLoading;
        private readonly IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel;
        private readonly Settings appEntrySettings;
        private readonly LifetimeScope _lifetimeScope;
        private readonly IOpenRoomCmd openRoomCmd;
        private bool isDisposed;
        private SimpleCommand getSpaceCommand;
        private InteractionRequest reloadDataRequest;
        private CancellationTokenSource getSpaceGroupCancellation;
        private ObservableList<SpaceGroupCellViewModel> items = new ObservableList<SpaceGroupCellViewModel>();

        public SpaceGroupListViewModel(
            ILoggerFactory loggerFactory,
            IService spaceService,
            IPublisher<SceneFlow.ChangeScene> pubSceneLoading,
            IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope,
            IOpenRoomCmd openRoomCmd)
        {
            getSpaceCommand = new SimpleCommand(OnGetSpaceCommand);
            reloadDataRequest = new InteractionRequest();
            this.loggerFactory = loggerFactory;
            this.spaceService = spaceService;
            this.pubSceneLoading = pubSceneLoading;
            this.pubLoadContentLevel = pubLoadContentLevel;
            this.appEntrySettings = appEntrySettings;
            this._lifetimeScope = lifetimeScope;
            this.openRoomCmd = openRoomCmd;
        }

        ~SpaceGroupListViewModel()
        {
            Dispose(false);
        }

        public ObservableList<SpaceGroupCellViewModel> Items => items;

        public ICommand GetSpaceCommand => getSpaceCommand;

        public IInteractionRequest ReloadDataRequest => reloadDataRequest;

        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var item in Items)
                {
                    item?.Dispose();
                }

                Items.Clear();

                if (getSpaceGroupCancellation != null)
                {
                    getSpaceGroupCancellation.Cancel();
                    getSpaceGroupCancellation.Dispose();
                }
            }

            base.Dispose(disposing);

            isDisposed = true;
        }

        private void OnGetSpaceCommand()
        {
            if (getSpaceGroupCancellation != null)
            {
                getSpaceGroupCancellation.Cancel();
                getSpaceGroupCancellation.Dispose();
            }

            getSpaceGroupCancellation = new CancellationTokenSource();
            GetSpaceGroupAsync(getSpaceGroupCancellation.Token).Forget();
        }

        private async UniTaskVoid GetSpaceGroupAsync(CancellationToken token)
        {
            var groups = await spaceService.GetSpaceGroups(token);
            var groupViewModels = groups.Select(group => new SpaceGroupCellViewModel(
                group,
                spaceService,
                pubSceneLoading,
                pubLoadContentLevel,
                appEntrySettings,
                _lifetimeScope,
                openRoomCmd,
                loggerFactory)).ToList();
            foreach (var item in Items)
            {
                item?.Dispose();
            }

            Items.Clear();

            Items.AddRange(groupViewModels);

            reloadDataRequest?.Raise();
        }
    }
}
