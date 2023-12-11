using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using Microsoft.Extensions.Logging;
using TPFive.Game.Login;
using UniRx;
using UnityEngine;
using VContainer;

namespace TPFive.Room
{
    public sealed partial class FusionRoom : IRoom
    {
        public const string PropertyNameSpaceID = "space_id";

        [Inject]
        private RoomSetting roomSetting;
        [Inject]
        private INetworkSceneManager netSceneMgr;
        [Inject]
        private INetworkObjectPool netObjectPool;
        [Inject]
        private ILoggerFactory loggerFactory;
        [Inject]
        private Service service;
        [Inject]
        private IObjectResolver objectResolver;

        private ILogger<FusionRoom> logger;

        private NetworkRunner fusionRunner;

        private ushort? port;

        private IDisposable retryStartGameRoutineSub;

        private IPlayerSpawner playerSpawner;

        private bool isDisposed;

        public event Action OnStarted;

        public string SpaceID { get; private set; }

        public PhotonRegion Region { get; private set; }

        public string RoomID { get; private set; }

        public GameMode Mode { get; private set; }

        public string SceneKey { get; private set; }

        public bool IsInRetryStartGame => retryStartGameRoutineSub != null;

        public ILogger<FusionRoom> Logger => logger ??= loggerFactory.CreateLogger<FusionRoom>();

        void IRoom.Open(GameMode mode, string spaceID, string roomID, string sceneKey, PhotonRegion region, ushort? port)
        {
            // Verify the given space id has value.
            if (string.IsNullOrEmpty(spaceID))
            {
                throw new SpaceException("Empty space id");
            }

            // Verify the given scene addressable key has value.
            if (string.IsNullOrEmpty(sceneKey))
            {
                throw new SpaceException("Empty scene addressable key");
            }

            Mode = mode;
            SpaceID = spaceID;
            RoomID = roomID;
            SceneKey = sceneKey;
            Region = region;
            this.port = port;
            _ = StartFusion(isRetry: false);
        }

        void IRoom.Close()
        {
        }

        void IDisposable.Dispose()
        {
            if (!isDisposed)
            {
                Release();
                GC.SuppressFinalize(this);
                isDisposed = true;
            }
        }

        private AuthenticationValues GetAuthenticationJson(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Logger.LogError("{Method}: Token is null or empty.", nameof(GetAuthenticationJson));

                return default;
            }

            try
            {
                var authentication = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom, };
                authentication.SetAuthPostData(new Dictionary<string, object>(1)
                {
                    {
                        "authorization", token
                    },
                });
                return authentication;
            }
            catch (Exception e)
            {
                Logger.LogError("{Method}: Failed {Exception}", nameof(GetAuthenticationJson), e);

                return default;
            }
        }

        private async Task StartFusion(bool isRetry)
        {
            if (fusionRunner != null)
            {
                // Fusion is being launching or has been launched.
                return;
            }

            if (!isRetry && IsInRetryStartGame)
            {
                // The retry-start-game routine has been undergoing.
                return;
            }

            // Create Fusion NetworkRunner
            fusionRunner = new GameObject($"fusion_room_{Mode}_{SpaceID}_{SceneKey}").AddComponent<NetworkRunner>();

            // Set up fusion-launching arguments
            StartGameArgs args = default;
            args.SessionName = RoomID;
            args.GameMode = Mode;
            args.IsOpen = false;    // Prevent clients from entering the session until its scene has been loaded.
            args.SessionProperties = CreateSessionProperties();
            args.SceneManager = netSceneMgr;
            args.ObjectPool = netObjectPool;
            args.CustomPhotonAppSettings = CreatePhotonAppSettings();
            if (roomSetting.CustomAuthentication)
            {
                args.AuthValues = GetAuthenticationJson(service.GetAccessToken());
            }

            if (port.HasValue)
            {
                args.Address = NetAddress.Any(port.Value);
            }

            args.Initialized = OnFusionInitialized;

            // Start Fusion session asynchronously
            var result = await fusionRunner.StartGame(args);
            if (result.Ok)
            {
                Logger.LogInformation("Start Fusion session successfully at {Region}", fusionRunner.SessionInfo.Region);
                StopRetryStartGame();
            }
            else
            {
                // Note: The newly-created NetworkRunner is destroyed automatically by now when starting-game failure occurs.
                Logger.LogError("Failed starting Fusion session: {ErrMsg}", result.ErrorMessage);
                StartRetryStartGame();
            }
        }

        private void OnFusionInitialized(NetworkRunner runner)
        {
            runner.AddCallbacks(this);

            // Spawn an instance of NetSceneController on server side to control the scene loading.
            if (runner.IsServer)
            {
                runner.Spawn(roomSetting.NetSceneCtrlPrefab, Vector3.zero, Quaternion.identity, onBeforeSpawned: InitNetSceneCtrl);
            }
        }

        private AppSettings CreatePhotonAppSettings()
        {
            var appSettings = PhotonAppSettings.Instance.AppSettings.GetCopy();
            appSettings.FixedRegion = Region.ToName();
            return appSettings;
        }

        private Dictionary<string, SessionProperty> CreateSessionProperties()
        {
            var properties = new Dictionary<string, SessionProperty>();

            // Set session property 'SpaceID'
            if (!string.IsNullOrEmpty(SpaceID))
            {
                properties[PropertyNameSpaceID] = SpaceID;
            }

            return properties;
        }

        private IPlayerSpawner CreatePlayerSpawner(NetworkRunner networkRunner)
        {
            if (this.playerSpawner == null)
            {
                var playerSpawner = new PlayerSpawner(networkRunner);
                objectResolver.Inject(playerSpawner);
                this.playerSpawner = playerSpawner;
            }

            return this.playerSpawner;
        }

        private void InitNetSceneCtrl(NetworkRunner netRunner, NetworkObject netObj)
        {
            if (!netObj.TryGetComponent<NetSceneController>(out var netSceneCtrl))
            {
                throw new Exception($"{nameof(NetSceneController)} is lost");
            }

            netSceneCtrl.TargetSceneKey = SceneKey;
        }

        private void StartRetryStartGame()
        {
            // Check if the routine is still running.
            if (retryStartGameRoutineSub != null)
            {
                return;
            }

            // Start the routine.
            Logger.LogInformation("Start retrying to start Fusion session");
            retryStartGameRoutineSub = UniRx.ObservableExtensions.Subscribe(
                Observable.Interval(TimeSpan.FromSeconds(roomSetting.RetryStartGameInterval)).ObserveOnMainThread(),
                (unit) => _ = StartFusion(isRetry: true));
        }

        private void StopRetryStartGame()
        {
            // Check if the routine is still running.
            if (retryStartGameRoutineSub == null)
            {
                return;
            }

            // Stop the routine.
            Logger.LogInformation("Stop retrying to start Fusion session");
            retryStartGameRoutineSub.Dispose();
            retryStartGameRoutineSub = null;
        }

        private void Release()
        {
            // Destroy Fusion NetworkRunner
            if (fusionRunner != null)
            {
                // Fusion currently only has asynchronous shutdown method.
                // Not sure if try catch is necessary if shutdown throw exception. Even if
                // catch exception, how to deal it?
                _ = fusionRunner.Shutdown(true);
            }

            // Dispose PlayerSpawner
            playerSpawner?.Dispose();
            playerSpawner = null;

            // Stop retry-start-game routine
            StopRetryStartGame();
        }
    }
}
