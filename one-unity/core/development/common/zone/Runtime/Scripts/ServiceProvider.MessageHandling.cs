using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game;
using UniRx;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace TPFive.Extended.Zone
{
    using TPFive.Game.Logging;

    using GameMessages = TPFive.Game.Messages;

    /// <summary>
    /// This part deals with message handling setup.
    /// </summary>
    public sealed partial class ServiceProvider
    {
        // This default order is for room and home, not content
        private const int CategoryOrder = 5;
        private const int SubCategoryOrder = 0;
        private const string RoomSceneKey = "Entry/Scenes/Room - Entry.unity";
        private const string HomeSceneKey = "Entry/Scenes/Home - Entry - Mobile.unity";

        private int _roomCategoryOrder;
        private int _roomSubCategoryOrder;
        private int _homeCategoryOrder;
        private int _homeSubCategoryOrder;
        private string _roomSceneKey;
        private string _homeSceneKey;

        private async UniTask SetupMessageHandling(CancellationToken cancellationToken = default)
        {
            // TODO: Should add avatar editor scene later.
            _subBackToHome
                .Subscribe(async x =>
                {
                    Logger.LogEditorDebug("{Method} receive BackToHome", nameof(SetupMessageHandling));
                })
                .AddTo(_compositeDisposable);

            _subLoadContentLevel
                .Subscribe(async x =>
                {
                    await HandleLoadRoom(x, cancellationToken);
                })
                .AddTo(_compositeDisposable);

            _subUnloadContentLevel
                .Subscribe(async x =>
                {
                    HandleUnloadContentLevel(cancellationToken);
                })
                .AddTo(_compositeDisposable);

            await UniTask.CompletedTask;
        }

        private async Task HandleLoadRoom(GameMessages.LoadContentLevel x, CancellationToken cancellationToken)
        {
            Logger.LogEditorDebug("{Method} receive LoadContentLevel", nameof(HandleLoadRoom));

            // The code is extracted from UI view model, need to be adjusted.
            _openRoomCmd.Set(x.Title, x.LevelBundleId);
            await ChangeScene(HomeSceneKey, RoomSceneKey, cancellationToken);
        }

        private void HandleUnloadContentLevel(CancellationToken cancellationToken)
        {
            Logger.LogEditorDebug("{Method} receive UnloadContentLevel", nameof(HandleUnloadContentLevel));

            _pubNotifyNetLoaderToUnload.Publish(new GameMessages.NotifyNetLoaderToUnload
            {
                OnCompleteAction = async () =>
                {
                    await ChangeScene(RoomSceneKey, HomeSceneKey, cancellationToken);
                }
            });
        }

        // TODO: Add error handling if possible
        private async UniTask ChangeScene(
            string fromSceneKey,
            string toSceneKey,
            CancellationToken cancellationToken = default)
        {
            var result = await _sceneFlowService.UnloadSceneAsync(
                fromSceneKey,
                CategoryOrder,
                SubCategoryOrder,
                false,
                cancellationToken);

            if (!result)
            {
                Logger.LogWarning("{Method} unload {SceneKey} fail", nameof(ChangeScene), fromSceneKey);
            }

            var scene = await _sceneFlowService.LoadSceneAsync(
                toSceneKey,
                LoadSceneMode.Additive,
                CategoryOrder,
                SubCategoryOrder,
                default,
                false,
                cancellationToken);

            if (!scene.IsValid())
            {
                Logger.LogWarning("{Method} load {SceneKey} fail", nameof(ChangeScene), toSceneKey);
            }
        }
    }
}
