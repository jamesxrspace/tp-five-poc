using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.Avatar.Factory;
using TPFive.Game.Record.Entry.Settings;
using TPFive.Game.Record.Scene;
using TPFive.Game.Record.Scene.SpawnPoint;
using TPFive.Game.Reel;
using TPFive.Game.Reel.Camera;
using TPFive.Game.Utils;
using TPFive.OpenApi.GameServer;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;
using XRSpace.OpenAPI;
using AppEntrySetting = TPFive.Game.App.Entry.Settings;
using ICameraService = TPFive.Game.Camera.IService;
using IDecorationService = TPFive.Game.Decoration.IService;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IRecordService = TPFive.Game.Record.IService;
using IReelSceneService = TPFive.Game.Record.Scene.IService;
using IReelService = TPFive.Game.Reel.IService;
using IResourceService = TPFive.Game.Resource.IService;
using ISceneFlowService = TPFive.Game.SceneFlow.IService;
using IUserService = TPFive.Game.User.IService;

namespace TPFive.Game.Record.Entry
{
    public partial class ReelManager : MonoBehaviour
    {
        private const int FilmWidth = 750;
        private const int FilmHeight = 1334;
        private const int FilmFrameRate = 30;
        private IRecordService recordService;
        private IResourceService resourceService;
        private ISceneFlowService sceneFlowService;
        private IReelService reelService;
        private IReelSceneService reelSceneService;
        private ILoggerFactory loggerFactory;
        private ISpawnPointService spawnPointService;
        private ILogger log;
        private IAssetApi assetApi;
        private IServerBaseUriProvider serverBaseUriProvider;
        private ICameraService cameraService;
        private IDecorationService decorationService;
        private AssetAccessHelper assetAccessHelper;
        private MusicToMotionService musicToMotionService;
        private ReelLoader loader;
        private ReelCameraController cameraController;
        private MusicController musicController;
        private LifetimeScope lifetimeScope;
        private CancellationTokenSource cancellationTokenSource;
        private AppEntrySetting appEntrySettings;
        private ISubscriber<Messages.SceneLoaded> subSceneLoaded;
        private List<ReelPlayer> playlistPlayers = new List<ReelPlayer>();
        private ReelPlayer userPlayer;
        private SessionStartOption sessionStartOption;
        private RecordData[] sourceFootage;
        private ReelDirectorConfig reelDirectorConfig;
        private ReelDirector reelDirector;
        private ReelSceneInfo reelSceneInfo;
        private ReelSceneDesc reelSceneDesc;
        private ReelAttachmentSettings reelAttachmentSettings;

        // TBD: remove this after [TF3R-128] is implemented. a workaround for replay recorddata.
        private string audioFilePath;

        public bool IsSessionRunning => machine.State == Mode.Recording;

        public RecordData[] Footage => sourceFootage;

        public ReelSceneInfo ReelSceneInfo => reelSceneInfo;

        public static float GetFootageLength(RecordData[] footage)
        {
            if (footage == null || footage.Length == 0f)
            {
                return 0;
            }

            var motionFootage = footage.OfType<AvatarRecordData>().ToArray();
            if (motionFootage.Length == 0)
            {
                return 0;
            }

            return motionFootage.Max(x => x.GetLengthSec());
        }

        [Inject]
        public void Construct(
            IAvatarFactory avatarFactory,
            IAssetApi assetApi,
            IRecordService recordService,
            IResourceService resourceService,
            IUserService userService,
            ISceneFlowService sceneFlowService,
            ICameraService cameraService,
            IReelService reelService,
            IReelSceneService reelSceneService,
            ILoggerFactory loggerFactory,
            ISpawnPointService spawnPointService,
            IObjectResolver objectResolver,
            IServerBaseUriProvider serverBaseUriProvider,
            RecordSetting recordSetting,
            LifetimeScope lifetimeScope,
            ISubscriber<TPFive.Game.Messages.SceneLoaded> subSceneLoaded,
            AppEntrySetting appEntrySettings,
            ReelDirectorConfig reelDirectorConfig,
            MusicToMotionService musicToMotionService,
            IDecorationService decorationService,
            ReelAttachmentSettings reelAttachmentSettings)
        {
            this.assetApi = assetApi;
            this.recordService = recordService;
            this.resourceService = resourceService;
            this.sceneFlowService = sceneFlowService;
            this.lifetimeScope = lifetimeScope;
            this.loggerFactory = loggerFactory;
            this.reelDirectorConfig = reelDirectorConfig;
            this.spawnPointService = spawnPointService;
            this.reelService = reelService;
            this.reelSceneService = reelSceneService;
            this.subSceneLoaded = subSceneLoaded;
            this.appEntrySettings = appEntrySettings;
            this.musicToMotionService = musicToMotionService;
            this.serverBaseUriProvider = serverBaseUriProvider;
            this.cameraService = cameraService;
            this.decorationService = decorationService;
            this.reelAttachmentSettings = reelAttachmentSettings;

            log = loggerFactory.CreateLogger<ReelManager>();
            loader = CreateLoader(
                new ReelAvatarLoader(
                    avatarFactory,
                    userService,
                    loggerFactory,
                    objectResolver,
                    decorationService,
                    recordSetting.LocalAvatarPrefab));
            cameraController = objectResolver.Resolve<ReelCameraController>();
            musicController = new MusicController(loggerFactory);
            assetAccessHelper = new AssetAccessHelper(loggerFactory, assetApi);
            InitStateMachine();
        }

        public async UniTask Setup(ReelSetupOption option)
        {
            async UniTask<ReelSceneInfo> LoadAsset(Stream stream)
            {
                var (avatarRecordData, audioRecordData, sceneRecordData) = await loader.CreateRecordData(stream);
                var sceneInfo = await loader.ReplaceScene(sceneRecordData.ToReelSceneDesc(), cancellationTokenSource.Token);
                await SetupFootage(Enumerable.Concat(avatarRecordData.Cast<RecordData>(), audioRecordData.Cast<RecordData>()).ToArray());
                return sceneInfo;
            }

            option.Validate();
            machine.Fire(ModeCommand.Load);

            try
            {
                if (!string.IsNullOrEmpty(option.XrsFilePath))
                {
                    using Stream fileStream = File.OpenRead(option.XrsFilePath);
                    reelSceneInfo = await LoadAsset(fileStream);
                }
                else if (!string.IsNullOrEmpty(option.ReelUrl))
                {
                    // download file from url
                    byte[] downloadData = await assetAccessHelper.Download(
                        option.ReelUrl,
                        maxRetries: 3,
                        connectionTimeoutSeconds: 10,
                        requestTimeoutSeconds: 10,
                        Progress.Create<float>(
                            progress =>
                                log.LogDebug($"{nameof(Setup)} Download {option.ReelUrl} {progress * 100} %")),
                        destroyCancellationToken);

                    using MemoryStream memoryStream = new MemoryStream(downloadData);
                    reelSceneInfo = await LoadAsset(memoryStream);
                }
                else
                {
                    reelSceneInfo = await loader.ReplaceScene(option.SceneDesc, cancellationTokenSource.Token);
                    reelSceneDesc = option.SceneDesc;
                }

                userPlayer = await LoadUserPlayer(option.ShowUserAvatar, cancellationTokenSource.Token);

                var initialReelState = reelSceneInfo.EnablePrepareState ? ReelState.Prepare : ReelState.Standby;
                if (ShouldUseUserPlayer(initialReelState))
                {
                    SetupAvatarPlacement(userPlayer, playlistPlayers.Count);
                }

                SetupState(initialReelState);

                reelDirector = new ReelDirector(
                    reelDirectorConfig,
                    reelSceneInfo,
                    loggerFactory);

                machine.Fire(ModeCommand.Loaded);
            }
            catch (OperationCanceledException)
            {
                log.LogDebug("Setup canceled");
            }
            catch (Exception e)
            {
                log.LogError($"Setup failed, {e}");
                throw;
            }
        }

        public async UniTask SetupFootage(RecordData[] footage)
        {
            if (footage == null)
            {
                loader.UnloadPlayers(playlistPlayers);
                playlistPlayers = new List<ReelPlayer>();
            }
            else
            {
                bool reloadNeeded = !IsSameFootage(sourceFootage, footage);
                if (reloadNeeded)
                {
                    loader.UnloadPlayers(playlistPlayers);
                    playlistPlayers = await loader.CreateAvatarWithMotion(footage.OfType<AvatarRecordData>().ToList(), cancellationTokenSource.Token);
                }

                // TBD: remove this after 'pause at first frame' is implemented.
                for (int i = 0; i < playlistPlayers.Count; ++i)
                {
                    SetupAvatarPlacement(playlistPlayers[i], i);
                }
            }

            sourceFootage = footage;
        }

        public async UniTask StartSession(SessionStartOption option)
        {
            machine.Fire(ModeCommand.Record);
            sessionStartOption = option;
            userPlayer.Root.SetActive(option.JoinUserAvatar);

            if (option.EnableBgm)
            {
                musicController.Upsert("bgm", option.BgmClip).Play();
            }

            if (option.EnableRecordMotion)
            {
                var recordData = PrepareRecordData(option);
                var (success, error) = StartRecord(recordData.ToArray());
                if (!success)
                {
                    log.LogError($"Start record failed, {error}");
                    machine.Fire(ModeCommand.Finish);
                    return;
                }
            }

            if (option.EnableMusicToMotion)
            {
                await SetupMusicToMotionAigc();
            }

            if (sourceFootage != null)
            {
                sourceFootage.OfType<AvatarRecordData>().ToList().ForEach(avatarRecordData => avatarRecordData.ApplyMotionToAvatarModel(0));

                PlayFootage(sourceFootage, option.PlaybackFinishedHandler, option.EnableRecordMotion).Forget();
            }

            // We should setup state after all avatars are placed correctly, since the
            // initial position of the active camera may depend on the pose of avatars,
            // such as move avatar to spwan point, selfie camera follow target avatar...etc
            SetupState(option.State);

            if (option.EnableMusicToMotion)
            {
                PlayGeneratedMotion(option.MusicMotion);
            }

            async UniTask SetupMusicToMotionAigc()
            {
                if (musicToMotionService.IsReady)
                {
                    return; // Had set up
                }

                if (userPlayer == null)
                {
                    throw new ReelException("User player was not loaded.");
                }

                await musicToMotionService.CreateMotionPlayer();

                HumanPoseSynchronizer.Enabled = true;
                HumanPoseSynchronizer.SetHumanPoseProvider(() => musicToMotionService.CapturedHumanPose);
            }
        }

        public async UniTask<string> StopSession(SessionEndOption option)
        {
            RecordData[] GetMergedFootage()
            {
                var sourceFootageClone = sourceFootage ?? Array.Empty<RecordData>();
                return sourceFootageClone.Concat(recordService.GetRecordData()).ToArray();
            }

            string motionFilePath = string.Empty;
            machine.Fire(ModeCommand.Finish);

            log.LogDebug($"Stop record.");

            // Stop bgm if it's not join mode, as background footage might have a longer duration.
            var bgmAudioSource = musicController.Find("bgm");

            if (bgmAudioSource != null && bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Stop();
            }

            if (sessionStartOption.EnableMusicToMotion)
            {
                StopGeneratedMotion();
            }

            if (sessionStartOption.EnableRecordMotion)
            {
                recordService.StopRecord();

                if (option.EnableExport)
                {
                    option.Footage = GetMergedFootage();
                }

                if (option.SaveToFile)
                {
                    motionFilePath = await SaveRecord(GetMergedFootage());
                }
            }

            if (sourceFootage != null && recordService.IsPlaying())
            {
                recordService.StopPlay();
            }

            sessionStartOption = null;
            return motionFilePath;
        }

        public async UniTask<string> SaveRecord(RecordData[] rawFootage)
        {
            if (rawFootage == null)
            {
                throw new InvalidOperationException("No record data is cached");
            }

            if (machine.IsInState(Mode.Recording))
            {
                throw new InvalidOperationException("Record is not finished");
            }

            string motionFilePath;
            (motionFilePath, audioFilePath) = await recordService.ExportToFile(rawFootage);
            log.LogDebug($"SaveRecord(): Motion file saved at {motionFilePath}, audio file saved at: {audioFilePath}");
            return motionFilePath;
        }

        public async UniTask<(string thumbnailPath, string filmPath)> CreateFilm(RecordData[] footage, CancellationToken token)
        {
            if (machine.IsInState(Mode.Recording))
            {
                throw new Exception("Record is not finished.");
            }

            AudioSource filmAudioSource = null;
            if (!string.IsNullOrEmpty(audioFilePath))
            {
                var clip = await MediaFileUtility.CreateClipFromFile(audioFilePath, AudioType.WAV);
                filmAudioSource = musicController.Upsert("film", clip);
            }

            recordService.StartFilm(FilmWidth, FilmHeight, FilmFrameRate, filmAudioSource, CameraCache.Main);

            bool isPlaybackEnd = false;
            recordService.StartPlay(footage.OfType<AvatarRecordData>().ToArray(), _ => isPlaybackEnd = true);

            try
            {
                await UniTask.WaitUntil(() => isPlaybackEnd, cancellationToken: token);
                (string thumbnailPath, string filmFilePath) = await recordService.StopFilm();

                log.LogDebug($"SaveFilm(): Film file saved at cache :{filmFilePath} / thumbnail file saved at cache :{thumbnailPath}");
                musicController.DestroyAudioSource("film");
                return (thumbnailPath, filmFilePath);
            }
            catch (OperationCanceledException)
            {
                log.LogDebug("Start film canceled");
                return (null, null);
            }
        }

        public void JoinUser()
        {
            userPlayer.Root.SetActive(true);
            SetupAvatarPlacement(userPlayer, playlistPlayers.Count);
        }

        public void EndRecordReel()
        {
            loader.BackToLobby().Forget();
        }

        public async UniTask<RecordData[]> ReadFootageFromFile(string filePath)
        {
            using FileStream stream = File.OpenRead(filePath);
            var (avatarRecordDataList, audioRecordDataList, sceneRecordData) = await loader.CreateRecordData(stream);

            return Enumerable.Concat(avatarRecordDataList.Cast<RecordData>(), audioRecordDataList.Cast<RecordData>()).ToArray();
        }

        public void PlayBGM(AudioClip audioClip)
        {
            musicController.Upsert("bgm", audioClip).Play();
        }

        public void StopBGM()
        {
            var bgmAudioSource = musicController.Find("bgm");
            if (bgmAudioSource != null)
            {
                bgmAudioSource.Stop();
            }
        }

        /// <summary>
        /// Setup state for reel scene.
        /// should be called after scene and avatars loaded.
        /// </summary>
        /// <param name="state">state of reel</param>
        public void SetupState(ReelState state)
        {
            if (reelSceneInfo == null)
            {
                log.LogError("{Method}: reelSceneInfo is null", nameof(SetupState));
                return;
            }

            log.LogDebug("{Method}: Setup {State} state", nameof(SetupState), state);

            if (state == ReelState.Prepare && !reelSceneInfo.EnablePrepareState)
            {
                // Auto move next state if prepare state is not enabled.
                state = ReelState.Standby;
                log.LogInformation(
                    "{Method}: {CurrentState} state be disabled, so auto move to {NextState} state",
                    nameof(SetupState),
                    ReelState.Prepare,
                    state);
            }

            var stateSetting = reelSceneInfo.GetSettingByState(state);

            bool shouldUseUserPlayer = ShouldUseUserPlayer(state);
            int playerIndex = shouldUseUserPlayer ? playlistPlayers.Count : playlistPlayers.Count - 1;
            var cameraSetting = GetCameraSettingFromState(stateSetting, playerIndex);
            cameraController.TurnOnCamera(cameraSetting, shouldUseUserPlayer ? userPlayer.Root : playlistPlayers[^1].Root);
        }

        public async UniTask DiscardRunningSession()
        {
            if (IsSessionRunning)
            {
                await StopSession(SessionEndOption.Drop());
            }
        }

        protected void OnDestroy()
        {
            ResetCurrentTrack();
            cameraController.TurnOffCamera();
            if (reelDirector != null)
            {
                reelDirector.Dispose();
                reelDirector = null;
            }

            if (trackingManager != null)
            {
                trackingManager.OnFaceTrackingStarted -= OnFaceTrackingStarted;
                trackingManager.OnBodyTrackingStarted -= OnBodyTrackingStarted;
                trackingManager.OnLossTrackingChanged -= OnLossTrackingChanged;
            }

            if (musicController != null)
            {
                musicController.Dispose();
                musicController = null;
            }
        }

        private UniTask PlayFootage(RecordData[] recordData, Action<bool> callback, bool playInBackground = false)
        {
            var uct = new UniTaskCompletionSource();
            try
            {
                Action<bool> localCallback = (result) =>
                {
                    callback?.Invoke(result);
                    _ = uct.TrySetResult();

                    if (!playInBackground)
                    {
                        machine.Fire(ModeCommand.Finish);
                    }
                };

                recordService.StartPlay(recordData, localCallback);
            }
            catch (OperationCanceledException)
            {
                log.LogDebug("Start play canceled");
                uct.TrySetCanceled();
            }
            catch (Exception e)
            {
                log.LogError($"Play record failed, {e}");
                uct.TrySetException(e);
            }

            return uct.Task;
        }

        private (bool success, string error) StartRecord(RecordData[] recordData)
        {
            log.LogDebug($"Start recording.");
            var result = recordService.StartRecord(recordData);
            return (result.Ok, result.Error);
        }

        private ReelLoader CreateLoader(ReelAvatarLoader reelAvatarLoader)
        {
            return new ReelLoader(
                recordService,
                sceneFlowService,
                resourceService,
                reelSceneService,
                loggerFactory,
                reelAvatarLoader,
                lifetimeScope,
                appEntrySettings,
                subSceneLoaded);
        }

        private UniTask UnloadAssets()
        {
            loader.UnloadPlayer(userPlayer);
            loader.UnloadPlayers(playlistPlayers);
            return loader.UnloadCurrentScene(cancellationTokenSource.Token);
        }

        private void RenewCancel()
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
            }

            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        }

        private List<RecordData> PrepareRecordData(SessionStartOption option)
        {
            Assert.IsTrue(option.EnableRecordMotion, "PrepareRecordData should be called in recording session");
            var recordData = new List<RecordData>();
            loader.MakeAvatarRecordable(new ReelPlayer[] { userPlayer }, recordData);

            if (option.EnableRecordVoice)
            {
                var recordDuration = option.Duration;
                var data = new MicAudioRecordData(recordDuration);
                var micAudioSource = musicController.Upsert("mic", default);
                micAudioSource.volume = 1f;
                data.Bind(micAudioSource);
                recordData.Add(data);
            }

            if (option.EnableBgm)
            {
                var data = new AudioRecordData();
                var bgmAudioSource = musicController.Find("bgm");
                bgmAudioSource.volume = 0.2f;
                data.Bind(bgmAudioSource);
                recordData.Add(data);
            }

            var sceneRecordData = new SceneRecordData();
            sceneRecordData.FromReelSceneDesc(loader.CurSceneDesc);
            recordData.Add(sceneRecordData);

            return recordData;
        }

        private async UniTask<ReelPlayer> LoadUserPlayer(
            bool defaultVisibility,
            CancellationToken cancellationToken = default)
        {
            var player = await loader.LoadUserPlayer(cancellationToken);
            player.Root.SetActive(defaultVisibility);

            return player;
        }

        private bool IsSameFootage(RecordData[] source, RecordData[] other)
        {
            return source != null && other != null && source.SequenceEqual(other);
        }

        /// <summary>
        /// Get camera setting from state setting.
        /// </summary>
        /// <param name="stateSetting">current reel state setting</param>
        /// <param name="playerIndex">current host player index</param>
        /// <returns>return suitable reel camera setting</returns>
        private ReelCameraSetting GetCameraSettingFromState(ReelStateSetting stateSetting, int playerIndex)
        {
            if (stateSetting == null)
            {
                log.LogError("{Method}: stateSetting is null", nameof(GetCameraSettingFromState));
                return null;
            }

            if (stateSetting.EnableSingleDefaultCamera)
            {
                return stateSetting.SingleDefaultCameraSetting;
            }

            int multiDefaultCameraSettingsCount = stateSetting.MultiDefaultCameraSettings.Count;
            if (playerIndex < 0 || multiDefaultCameraSettingsCount == 0)
            {
                log.LogError(
                    "{Method}: player index({PlayerIndex}) is out of range({PointCount})",
                    nameof(GetCameraSettingFromState),
                    playerIndex,
                    multiDefaultCameraSettingsCount);
                return null;
            }

            playerIndex %= multiDefaultCameraSettingsCount;

            return stateSetting.MultiDefaultCameraSettings[playerIndex];
        }

        /// <summary>
        /// Check user avatar should be used in current reel state or not.
        /// </summary>
        /// <param name="state">current reel state</param>
        /// <returns>
        /// IF TRUE means user avatar should be used in current reel state,
        /// otherwise not.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Should not be called with unknown state.
        /// </exception>
        private bool ShouldUseUserPlayer(ReelState state)
        {
            return state switch
            {
                ReelState.Watch => false,
                ReelState.Prepare => true,
                ReelState.Standby => true,
                ReelState.Recording => true,
                ReelState.Preview => false,
                _ => throw new NotSupportedException($"Unknown state({state})")
            };
        }
    }
}
