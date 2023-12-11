using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using Microsoft.Extensions.Logging;
using TPFive.Game.App.Entry;
using TPFive.Game.Space;
using TPFive.Room;
using UnityEngine;
using UnityEngine.Networking;
using IService = TPFive.Game.Space.IService;
using LifetimeScope = TPFive.Home.Entry.LifetimeScope;

namespace TPFive.Game.Home.Entry
{
    public class SpaceGroupCellViewModel : ViewModelBase
    {
        private readonly SpaceListViewModel spaceListViewModel;
        private readonly IService spaceService;
        private readonly InteractionRequest calculateCellSizeRequest;
        private readonly SimpleCommand<float> setCellSizeCommand;
        private string id;
        private string spaceName;
        private string description;
        private string thumbnailUrl;
        private bool isExpanded;
        private bool disposed;
        private int dataIndex;
        private int cellIndex;
        private Texture thumbnailTexture;
        private SimpleCommand clickCommad;
        private Action<int, int> onClick;
        private CancellationTokenSource getSpacesCancellation;
        private Loxodon.Framework.Asynchronous.IAsyncResult loadTextureAsyncResult;

        public SpaceGroupCellViewModel(
            SpaceGroup group,
            IService spaceService,
            MessagePipe.IPublisher<SceneFlow.ChangeScene> pubSceneLoading,
            MessagePipe.IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope,
            IOpenRoomCmd openRoomCmd,
            ILoggerFactory loggerFactory)
            : this(
                group.Id,
                group.Name,
                group.Description,
                group.ThumbnailUrl,
                spaceService,
                pubSceneLoading,
                pubLoadContentLevel,
                appEntrySettings,
                lifetimeScope,
                openRoomCmd,
                loggerFactory)
            {
            }

        public SpaceGroupCellViewModel(
            string id,
            string spaceName,
            string description,
            string thumbnailUrl,
            IService spaceService,
            MessagePipe.IPublisher<SceneFlow.ChangeScene> pubSceneLoading,
            MessagePipe.IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope,
            IOpenRoomCmd openRoomCmd,
            ILoggerFactory loggerFactory)
        {
            this.id = id;
            this.spaceService = spaceService;
            SpaceName = spaceName;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            clickCommad = new SimpleCommand(OnClickCommand);
            calculateCellSizeRequest = new InteractionRequest();
            setCellSizeCommand = new SimpleCommand<float>(OnSetCellSizeCommand);
            Logger = Logging.Utility.CreateLogger<SpaceGroupCellViewModel>(loggerFactory);

            spaceListViewModel = new SpaceListViewModel(
                pubSceneLoading,
                pubLoadContentLevel,
                appEntrySettings,
                lifetimeScope,
                openRoomCmd,
                loggerFactory);
        }

        ~SpaceGroupCellViewModel()
        {
            Dispose(false);
        }

        public string SpaceName
        {
            get => spaceName;
            set => Set(ref spaceName, value, nameof(SpaceName));
        }

        public string Description
        {
            get => description;
            set => Set(ref description, value, nameof(Description));
        }

        public string ThumbnailUrl
        {
            get => thumbnailUrl;
            set => Set(ref thumbnailUrl, value, nameof(ThumbnailUrl));
        }

        public Texture ThumbnailTexture
        {
            get => thumbnailTexture;
            set => Set(ref thumbnailTexture, value, nameof(ThumbnailTexture));
        }

        public ICommand ClickCommand => clickCommad;

        public bool IsExpanded
        {
            get => isExpanded;
            set => Set(ref isExpanded, value, nameof(IsExpanded));
        }

        public float CellSize { get; set; }

        public IInteractionRequest CalculateCellSizeRequest => calculateCellSizeRequest;

        public ICommand SetCellSizeCommand => setCellSizeCommand;

        public SpaceListViewModel SpaceListViewModel => spaceListViewModel;

        public Microsoft.Extensions.Logging.ILogger Logger { get; }

        public void SetClickCallback(int dataIndex, int cellIndex, Action<int, int> onClick)
        {
            this.dataIndex = dataIndex;
            this.cellIndex = cellIndex;
            this.onClick = onClick;
        }

        public void CalculateCellSize()
        {
            calculateCellSizeRequest.Raise();
        }

        public void LoadTexture()
        {
            loadTextureAsyncResult?.Cancel();
            loadTextureAsyncResult = Executors.RunOnCoroutine(GetTextureByUnityWebRequest());
        }

        public void ReleaseTexture()
        {
            loadTextureAsyncResult?.Cancel();

            if (ThumbnailTexture != null)
            {
                UnityEngine.Object.Destroy(ThumbnailTexture);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                ReleaseTexture();

                if (getSpacesCancellation != null)
                {
                    getSpacesCancellation.Cancel();
                    getSpacesCancellation.Dispose();
                }

                spaceListViewModel?.Dispose();
            }

            disposed = true;

            base.Dispose(disposing);
        }

        private IEnumerator GetTextureByUnityWebRequest()
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(thumbnailUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Logger.LogWarning(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    ThumbnailTexture = DownloadHandlerTexture.GetContent(uwr);
                }
            }
        }

        private void OnSetCellSizeCommand(float size)
        {
            CellSize = size;
        }

        private void OnClickCommand()
        {
            if (getSpacesCancellation != null)
            {
                getSpacesCancellation.Cancel();
                getSpacesCancellation.Dispose();
            }

            getSpacesCancellation = new CancellationTokenSource();
            OnClickCommandAsync(getSpacesCancellation.Token).Forget();
        }

        private async UniTaskVoid OnClickCommandAsync(CancellationToken token)
        {
            IsExpanded = !IsExpanded;

            await CheckSpaceList(token);

            // reload scrollview
            onClick?.Invoke(dataIndex, cellIndex);
        }

        private async UniTask CheckSpaceList(CancellationToken token)
        {
            if (IsExpanded)
            {
                await ShowSpaceList(token);
            }
            else
            {
                HideSpaceList();
            }

            // recalculate size before reload scrollview
            CalculateCellSize();
        }

        private async UniTask ShowSpaceList(CancellationToken token)
        {
            try
            {
                var spaces = await spaceService.GetSpaces(id, token);
                token.ThrowIfCancellationRequested();

                // setup data
                spaceListViewModel.SetItems(spaces);
                spaceListViewModel.ShowCommand.Execute(null);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, "ShowSpaceList failed");
            }
        }

        private void HideSpaceList()
        {
            spaceListViewModel?.HideCommand.Execute(null);
        }
    }
}
