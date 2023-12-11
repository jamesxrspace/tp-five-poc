using System;
using System.Collections;
using System.Linq;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Execution;
using Loxodon.Framework.ViewModels;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.App.Entry;
using TPFive.Game.SceneFlow;
using TPFive.Room;
using UnityEngine;
using UnityEngine.Networking;
using LifetimeScope = TPFive.Home.Entry.LifetimeScope;

namespace TPFive.Game.Home.Entry
{
    using GameMessages = TPFive.Game.Messages;

    public class SpaceViewModel : ViewModelBase
    {
        private readonly string id;
        private readonly string sceneKey;
        private readonly IPublisher<ChangeScene> pubSceneLoading;
        private readonly IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel;
        private readonly Settings appEntrySettings;
        private readonly LifetimeScope _lifetimeScope;
        private readonly IOpenRoomCmd openRoomCmd;
        private string name;
        private string thumbnailUrl;
        private Texture2D thumbnailTexture;
        private SimpleCommand goToSpaceCommand;
        private Loxodon.Framework.Asynchronous.IAsyncResult loadTextureAsyncResult;
        private bool disposed;

        public SpaceViewModel(
            Space.Space space,
            IPublisher<ChangeScene> pubSceneLoading,
            IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope,
            IOpenRoomCmd openRoomCmd,
            ILoggerFactory loggerFactory)
             : this(
                 space.Id,
                 space.Name,
                 space.ThumbnailUrl,
                 space.SceneKey,
                 pubSceneLoading,
                 pubLoadContentLevel,
                 appEntrySettings,
                 lifetimeScope,
                 openRoomCmd,
                 loggerFactory)
        {
        }

        public SpaceViewModel(
            string id,
            string name,
            string thumbnailUrl,
            string sceneKey,
            IPublisher<ChangeScene> pubSceneLoading,
            IPublisher<Game.Messages.LoadContentLevel> pubLoadContentLevel,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope,
            IOpenRoomCmd openRoomCmd,
            ILoggerFactory loggerFactory)
        {
            this.id = id;
            this.Name = name;
            this.ThumbnailUrl = thumbnailUrl;
            this.sceneKey = sceneKey;
            this.pubSceneLoading = pubSceneLoading;
            this.pubLoadContentLevel = pubLoadContentLevel;
            this.appEntrySettings = appEntrySettings;
            this._lifetimeScope = lifetimeScope;
            this.openRoomCmd = openRoomCmd;
            goToSpaceCommand = new SimpleCommand(OnGoToSpaceCommand);
            Logger = Logging.Utility.CreateLogger<SpaceViewModel>(loggerFactory);
        }

        ~SpaceViewModel()
        {
            Dispose(false);
        }

        public Microsoft.Extensions.Logging.ILogger Logger { get; }

        public string Name
        {
            get => name;
            set => Set(ref name, value, nameof(Name));
        }

        public string ThumbnailUrl
        {
            get => thumbnailUrl;
            set => Set(ref thumbnailUrl, value, nameof(ThumbnailUrl));
        }

        public Texture2D ThumbnailTexture
        {
            get => thumbnailTexture;
            set => Set(ref thumbnailTexture, value, nameof(ThumbnailTexture));
        }

        public string HomeEntryTitle { get; set; }

        public string RoomEntryTitle { get; set; }

        public string Id => id;

        public ICommand GoToSpaceCommand => goToSpaceCommand;

        public void LoadTexture()
        {
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
            }

            disposed = true;

            base.Dispose(disposing);
        }

        private void OnGoToSpaceCommand()
        {
            goToSpaceCommand.Enabled = false;

            pubLoadContentLevel.Publish(new GameMessages.LoadContentLevel
            {
                Title = Id,
                LevelBundleId = sceneKey,
            });

            goToSpaceCommand.Enabled = true;
        }

        private SceneProperty GetSceneProperty(string title)
        {
            return appEntrySettings.ScenePropertyList
                .Where(x => x.title.Equals(title, StringComparison.Ordinal))
                .FirstOrDefault();
        }

        private IEnumerator GetTextureByUnityWebRequest()
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(thumbnailUrl))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Logger.LogWarning(request.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    ThumbnailTexture = DownloadHandlerTexture.GetContent(request);
                }
            }
        }
    }
}
