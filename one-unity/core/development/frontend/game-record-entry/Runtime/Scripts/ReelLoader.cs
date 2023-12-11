using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.Avatar;
using TPFive.Game.Avatar.Attachment;
using TPFive.Game.Decoration;
using TPFive.Game.Record.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using AppEntrySettings = TPFive.Game.App.Entry.Settings;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IRecordService = TPFive.Game.Record.IService;
using IReelSceneService = TPFive.Game.Record.Scene.IService;
using IResourceService = TPFive.Game.Resource.IService;
using ISceneFlowService = TPFive.Game.SceneFlow.IService;

namespace TPFive.Game.Record.Entry
{
    /// <summary>
    /// Load scene, avatar, music, decoration and other assets for recording.
    /// </summary>
    public class ReelLoader
    {
        private const int BlendShapeCount = 52;
        private const int LevelEntryCategoryOrder = 6;
        private const int LevelEntrySubOrder = 0;
        private const string ReelDecorationGroupId = nameof(ReelLoader);

        private readonly ISceneFlowService sceneFlowService;
        private readonly IRecordService recordService;
        private readonly IResourceService resourceService;
        private readonly IReelSceneService reelSceneService;
        private readonly ILogger log;
        private readonly ReelAvatarLoader avatarLoader;
        private readonly LifetimeScope lifetimeScope;

        private readonly ISubscriber<Messages.SceneLoaded> subSceneLoaded;

        private SceneProperty homeEntryProperty;
        private SceneProperty recordEntryProperty;
        private ReelSceneDesc curSceneDesc;

        public ReelLoader(
            IRecordService recordService,
            ISceneFlowService sceneFlowService,
            IResourceService resourceService,
            IReelSceneService reelSceneService,
            ILoggerFactory loggerFactory,
            ReelAvatarLoader avatarLoader,
            LifetimeScope lifetimeScope,
            AppEntrySettings appEntrySettings,
            ISubscriber<Messages.SceneLoaded> subSceneLoaded)
        {
            this.avatarLoader = avatarLoader;
            this.recordService = recordService;
            this.sceneFlowService = sceneFlowService;
            this.resourceService = resourceService;
            this.reelSceneService = reelSceneService;
            this.lifetimeScope = lifetimeScope;
            this.subSceneLoaded = subSceneLoaded;

            log = loggerFactory.CreateLogger<ReelLoader>();

            appEntrySettings.TryGetSceneProperty("HomeEntry", out homeEntryProperty);
            appEntrySettings.TryGetSceneProperty("RecordEntry", out recordEntryProperty);
        }

        public ReelSceneDesc CurSceneDesc => curSceneDesc;

        public UniTask<ReelPlayer> LoadUserPlayer(CancellationToken cancellationToken)
        {
            return avatarLoader.LoadUserPlayer(cancellationToken);
        }

        public UniTask<DecorationAttachment> CreateDecorationAttachment(string bundleId)
        {
            return avatarLoader.CreateDecorationAttachment(ReelDecorationGroupId, bundleId);
        }

        public UniTask<bool> DestroyDecorationAttachment(string bundleId, DecorationAttachment attachment)
        {
            return avatarLoader.DestroyDecorationAttachment(ReelDecorationGroupId, bundleId, attachment);
        }

        public bool TryApplyAttachmentToAvatar(DecorationAttachment attachment, string category, IAnchorPointProvider anchorPointProvider)
        {
            return avatarLoader.TryApplyAttachmentToAnchor(attachment, category, anchorPointProvider);
        }

        public UniTask<(List<AvatarRecordData>, List<AudioRecordData>, SceneRecordData)> CreateRecordData(string filePath)
        {
            using FileStream stream = File.OpenRead(filePath);
            return CreateRecordData(stream);
        }

        public async UniTask<(List<AvatarRecordData>, List<AudioRecordData>, SceneRecordData)> CreateRecordData(Stream stream)
        {
            var recordData = await recordService.LoadRecordAssets(stream);
            return (recordData.OfType<AvatarRecordData>().ToList(), recordData.OfType<AudioRecordData>().ToList(), recordData.OfType<SceneRecordData>().First());
        }

        // Create RecordAvatarData from Avatar and bind them
        public void MakeAvatarRecordable(IEnumerable<ReelPlayer> players, List<RecordData> result)
        {
            result.AddRange(players.Select(player =>
            {
                var data = new AvatarRecordData(Guid.NewGuid().ToString());
                BindAvatarWithRecordData(data, player);
                return data;
            }));
        }

        // Create Avatar from RecordAvatarData and bind them
        public async UniTask<List<ReelPlayer>> CreateAvatarWithMotion(List<AvatarRecordData> recordDataList, CancellationToken cancellationToken)
        {
            var playerList = await UniTask.WhenAll(recordDataList.Select(async recordData =>
            {
                var (avatarFromat, error) = AvatarFormat.Deserialize(recordData.GetAvatarFormat());
                if (error != null)
                {
                    log.LogError($"Load(): Load player failed, {error}");
                    return null;
                }

                var player = await avatarLoader.Load(avatarFromat, cancellationToken);
                player.Root.GetComponent<PlayerInputTransfer>().enabled = false;
                BindAvatarWithRecordData(recordData, player, recordData.GetAvatarFormat());
                return player;
            }));

            return playerList.Where(player => player != null).ToList();
        }

        public void UnloadPlayers(IEnumerable<ReelPlayer> players)
        {
            log.LogDebug("Unloading players...");
            foreach (var player in players)
            {
                player?.Dispose();
            }
        }

        public void UnloadPlayer(ReelPlayer player)
        {
            player?.Dispose();
        }

        public UniTask UnloadCurrentScene(CancellationToken cancellationToken)
        {
            return UnloadScene(curSceneDesc, cancellationToken);
        }

        public async UniTask UnloadScene(ReelSceneDesc sceneDesc, CancellationToken cancellationToken)
        {
            if (!sceneDesc.IsValid)
            {
                log.LogWarning("{Method}: Invalid scene desc. {SceneDesc}", nameof(UnloadScene), sceneDesc);
                return;
            }

            log.LogDebug("{Method}: Unloading scene({SceneName})...", nameof(UnloadScene), sceneDesc.Name);

            await (sceneDesc.IsBundleAsset
                ? UnloadBundleScene(sceneDesc, cancellationToken)
                : UnloadBuildInAddressableScene(sceneDesc, cancellationToken));

            log.LogDebug("{Method}: Unloaded scene({SceneName})", nameof(UnloadScene), sceneDesc.Name);
        }

        public async UniTask<ReelSceneInfo> LoadScene(ReelSceneDesc sceneDesc, CancellationToken cancellationToken)
        {
            if (!sceneDesc.IsValid)
            {
                log.LogError("{Method}: Invalid scene desc. {SceneDesc}", nameof(LoadScene), sceneDesc);
                return null;
            }

            log.LogDebug("{Method}: Loading scene({SceneName})...", nameof(LoadScene), sceneDesc.Name);

            var sceneInfo = await (sceneDesc.IsBundleAsset
                ? LoadBundleScene(sceneDesc, cancellationToken)
                : LoadBuildInAddressableScene(sceneDesc, cancellationToken));

            log.LogDebug("{Method}: Loaded scene({SceneName})", nameof(LoadScene), sceneDesc.Name);

            return sceneInfo;
        }

        public async UniTask<ReelSceneInfo> ReplaceScene(ReelSceneDesc sceneDesc, CancellationToken cancellationToken)
        {
            if (!sceneDesc.IsValid)
            {
                log.LogError("{Method}: Invalid scene desc. {SceneDesc}", nameof(ReplaceScene), sceneDesc);
                return null;
            }

            if (curSceneDesc != ReelSceneDesc.None)
            {
                await UnloadScene(curSceneDesc, cancellationToken);
            }

            var sceneInfo = await LoadScene(sceneDesc, cancellationToken);

            curSceneDesc = sceneDesc;

            return sceneInfo;
        }

        /// <summary>
        /// TBD [TF3R-190]:
        /// scene change event was not recorded in any form so far,
        /// so we always back to social lobby now.
        /// </summary>
        /// <returns>return a UniTask, when go back operation done will complete this task.</returns>
        public async UniTask BackToLobby()
        {
            // unload level scene(6-0) or build-in scene(5-1)
            await UnloadCurrentScene(CancellationToken.None);

            // from record scene(5-0) to home scene(4-0)
            sceneFlowService.ReplaceSceneAsync(
                recordEntryProperty.addressableKey,
                recordEntryProperty.categoryOrder,
                recordEntryProperty.subOrder,
                homeEntryProperty.addressableKey,
                homeEntryProperty.categoryOrder,
                homeEntryProperty.subOrder,
                null);
        }

        private async UniTask UnloadBundleScene(ReelSceneDesc sceneDesc, CancellationToken cancellationToken)
        {
            await sceneFlowService.UnloadSceneAsync(
                $"{sceneDesc.BundleID}.asset",
                LevelEntryCategoryOrder,
                LevelEntrySubOrder,
                true,
                cancellationToken);

            await resourceService.UnloadBundleDataAsync(sceneDesc.BundleID, cancellationToken);
        }

        private async UniTask UnloadBuildInAddressableScene(ReelSceneDesc sceneDesc, CancellationToken cancellationToken)
        {
            await sceneFlowService.UnloadSceneAsync(
                sceneDesc.BuildInAddressableKey,
                recordEntryProperty.categoryOrder,
                recordEntryProperty.subOrder + 1,
                false,
                cancellationToken);
        }

        private async UniTask<ReelSceneInfo> LoadBundleScene(
            ReelSceneDesc sceneDesc,
            CancellationToken cancellationToken)
        {
            var utcs = new UniTaskCompletionSource();
            var sceneLoadedSubscription = subSceneLoaded.Subscribe(OnLevelSubSceneLoaded);

            ReelSceneInfo prevSceneInfo = reelSceneService.SceneInfo.Value;
            ReelSceneInfo nextSceneInfo = null;
            var sceneInfoSubscription = reelSceneService.SceneInfo.Subscribe(OnReceivedSceneInfo);

            try
            {
                await resourceService.LoadBundledDataAsync(sceneDesc.BundleID, cancellationToken);

                _ = await sceneFlowService.LoadSceneAsync(
                    $"{sceneDesc.BundleID}.asset",
                    LoadSceneMode.Additive,
                    LevelEntryCategoryOrder,
                    LevelEntrySubOrder,
                    lifetimeScope,
                    true,
                    cancellationToken);

                // Wait level sub-scene loaded
                await utcs.Task;

                await UniTask.WaitUntil(
                    () => nextSceneInfo != null && nextSceneInfo != prevSceneInfo,
                    cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                log.LogInformation(
                    e,
                    "{Method}: Load bundle scene({BundleID}) canceled",
                    nameof(LoadScene),
                    sceneDesc.BundleID);
            }
            catch (Exception e)
            {
                log.LogError(
                    e,
                    "{Method}: Load bundle scene({BundleID}) failed",
                    nameof(LoadScene),
                    sceneDesc.BundleID);
            }
            finally
            {
                sceneLoadedSubscription.Dispose();
                sceneInfoSubscription.Dispose();
            }

            return nextSceneInfo;

            /// <summary>
            /// Callback when level sub-scene loaded.
            /// </summary>
            /// <param name="msg">scene loaded message.</param>
            void OnLevelSubSceneLoaded(TPFive.Game.Messages.SceneLoaded msg)
            {
                if (msg.CategoryOrder != LevelEntryCategoryOrder ||
                    msg.SubOrder <= LevelEntrySubOrder)
                {
                    return;
                }

                utcs.TrySetResult();
            }

            void OnReceivedSceneInfo(ReelSceneInfo info)
            {
                nextSceneInfo = info;
            }
        }

        private async UniTask<ReelSceneInfo> LoadBuildInAddressableScene(
            ReelSceneDesc sceneDesc,
            CancellationToken cancellationToken)
        {
            ReelSceneInfo prevSceneInfo = reelSceneService.SceneInfo.Value;
            ReelSceneInfo nextSceneInfo = null;
            var sceneInfoSubscription = reelSceneService.SceneInfo.Subscribe(OnReceivedSceneInfo);

            try
            {
                var scene = await sceneFlowService.LoadSceneAsync(
                    sceneDesc.BuildInAddressableKey,
                    LoadSceneMode.Additive,
                    recordEntryProperty.categoryOrder,
                    recordEntryProperty.subOrder + 1,
                    lifetimeScope,
                    false,
                    cancellationToken);

                SetActiveScene(scene);

                await UniTask.WaitUntil(
                    () => nextSceneInfo != null && nextSceneInfo != prevSceneInfo,
                    cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                log.LogInformation(
                    e,
                    "{Method}: Load build-in scene({AddressableKey}) canceled",
                    nameof(LoadScene),
                    sceneDesc.BuildInAddressableKey);
            }
            catch (Exception e)
            {
                log.LogError(
                    e,
                    "{Method}: Load build-in scene({AddressableKey}) failed",
                    nameof(LoadScene),
                    sceneDesc.BuildInAddressableKey);
            }
            finally
            {
                sceneInfoSubscription.Dispose();
            }

            return nextSceneInfo;

            void OnReceivedSceneInfo(ReelSceneInfo info)
            {
                nextSceneInfo = info;
            }
        }

        private void SetActiveScene(UnityEngine.SceneManagement.Scene scene)
        {
            if (!scene.IsValid())
            {
                throw new InvalidOperationException($"The scene({scene.name}) invalid.");
            }

            bool success = SceneManager.SetActiveScene(scene);

            log.LogInformation(
                "{Method}: Active scene({SceneName}) {ActiveState}",
                nameof(SetActiveScene),
                scene.name,
                success ? "successful" : "failure");
        }

        private void BindAvatarWithRecordData(AvatarRecordData recordAvatarData, ReelPlayer player, string avatarFormat = null)
        {
            recordAvatarData.Bind(
                player.Root,
                player.Avatar.Animator,
                player.Avatar.SkinManager.GetHeadSkinnedMeshRenderer(),
                BlendShapeCount,
                avatarFormat ?? AvatarFormat.Serialize(player.Avatar.AvatarFormat).Item1);
        }
    }
}
