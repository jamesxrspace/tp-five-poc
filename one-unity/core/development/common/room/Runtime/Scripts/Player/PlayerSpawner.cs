using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game;
using UniRx;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace TPFive.Room
{
    public sealed class PlayerSpawner : IPlayerSpawner
    {
        private static readonly TimeSpan PlayerDataWaitInterval = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan ExpiredPlayerCheckInterval = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Pending player is the player waiting for its data from client.
        /// </summary>
        private readonly ActiveList<PlayerRef> pendingPlayers = new (PlayerDataWaitInterval);
        private readonly NetworkRunner networkRunner;
        private ISpawnPointManager<PlayerSpawnPoint> spawnPointManager;
        private RoomSetting roomSetting;
        private ILogger logger;
        private IDisposable msgDisposable;
        private IDisposable routineDisposable;

        public PlayerSpawner(NetworkRunner networkRunner)
        {
            this.networkRunner = networkRunner;
        }

        [Inject]
        public void Construct(
            RoomSetting roomSetting,
            ISubscriber<EntityData<NetPlayerData>> playerDataSub,
            ILoggerFactory loggerFactory)
        {
            this.roomSetting = roomSetting;
            logger = loggerFactory.CreateLogger<PlayerSpawner>();
            msgDisposable = playerDataSub.Subscribe(OnPlayerDataReceived);
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
        {
            logger.LogInformation("Player({PlayerRef}) joined the room.", playerRef);

            // Start waiting for the player's data if it hasn't been spawned yet.
            if (networkRunner.GetPlayerObject(playerRef) == null)
            {
                StartWaitPlayerData(playerRef);
            }
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
        {
            logger.LogInformation("Player({PlayerRef}) left the room.", playerRef);

            // The player has either been spawned already, or waiting for its data.
            if (!DespawnPlayer(playerRef))
            {
                StopWaitPlayerData(playerRef);
            }
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
        {
            FetchSpawnPointManager();
        }

        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void Dispose()
        {
            StopSubscribeMsg();
            StopCheckPendingPlayer();
        }

        private void OnPlayerDataReceived(EntityData<NetPlayerData> entityData)
        {
            logger.LogInformation("Receive Player({PlayerRef})'s data.", entityData.PlayerRef);
            StopWaitPlayerData(entityData.PlayerRef);
            SpawnPlayer(entityData);
        }

        private NetworkObject SpawnPlayer(EntityData<NetPlayerData> entityData)
        {
            var playerRef = entityData.PlayerRef;
            var playerData = entityData.Data;

            // Check the given  player ref and the player prefab
            if (!playerRef.IsValid)
            {
                logger.LogError("{Method}: {Message}", nameof(SpawnPlayer), "the given player ref is invalid");
                return null;
            }

            if (roomSetting.PlayerPrefab == null)
            {
                logger.LogError("{Method}: {Message}", nameof(SpawnPlayer), "no PlayerPrefab set in RoomSetting");
                return null;
            }

            // Get spawning point for the given player
            var pointName = string.Empty;
            var pointPosition = Vector3.zero;
            var pointRotation = Quaternion.identity;

            if (!GetSpawnPoint(playerRef, out pointName, out pointPosition, out pointRotation))
            {
                logger.LogError("{Method}: {Message}", nameof(SpawnPlayer), "Failed fetching spawn position and rotation");
                return null;
            }

            // Spawn a new object for the given player
            var playerObject = networkRunner.Spawn(
                roomSetting.PlayerPrefab,
                pointPosition,
                pointRotation,
                playerRef,
                GetPlayerInitializer(entityData));
            logger.LogInformation("Spawned player {PlayerRef} at \"{PointName}\"", playerRef, pointName);

            // Register the player and its object with the NetworkRunner
            networkRunner.SetPlayerObject(playerRef, playerObject);

            // For AOI handling, make the player aware of its own player object in all cases.
            networkRunner.SetPlayerAlwaysInterested(playerRef, playerObject, true);

            return playerObject;
        }

        private bool DespawnPlayer(PlayerRef player)
        {
            // Despawn the object of the specified player
            var playerObject = networkRunner.GetPlayerObject(player);
            if (playerObject != null)
            {
                networkRunner.Despawn(playerObject);

                // Unregister the player with the NetworkRunner
                networkRunner.SetPlayerObject(player, null);
                networkRunner.SetPlayerAlwaysInterested(player, null, false);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FetchSpawnPointManager()
        {
            var managers = new List<ISpawnPointManager<PlayerSpawnPoint>>();
            networkRunner.SimulationUnityScene.FindObjectsOfTypeInOrder(managers);
            if (managers.Count > 0)
            {
                spawnPointManager = managers[0];
                spawnPointManager.CollectSpawnPoints();
            }
            else
            {
                logger.LogError("Failed fetching any SpawnPointManager in {SceneName}", networkRunner.SimulationUnityScene.name);
            }
        }

        private bool GetSpawnPoint(PlayerRef playerRef, out string pointName, out Vector3 pointPosition, out Quaternion pointRotation)
        {
            pointName = string.Empty;
            pointPosition = Vector3.zero;
            pointRotation = Quaternion.identity;

            if (spawnPointManager == null)
            {
                return false;
            }

            var pointTransform = spawnPointManager.GetNextSpawnPoint(playerRef, true);
            if (pointTransform == null)
            {
                return false;
            }

            pointName = pointTransform.name;
            pointPosition = pointTransform.position;
            pointRotation = pointTransform.rotation;
            return true;
        }

        private NetworkRunner.OnBeforeSpawned GetPlayerInitializer(EntityData<NetPlayerData> entityData)
        {
            return (netRunner, netObj) =>
            {
                if (netObj.TryGetComponent<PlayerAttributes>(out var playerAttributes))
                {
                    playerAttributes.UserId = entityData.Data.UserId;
                    playerAttributes.Nickname = entityData.Data.Nickname;
                }
                else
                {
                    throw new Exception($"Player prefab has no required component {nameof(PlayerAttributes)}");
                }
            };
        }

        private void StartWaitPlayerData(PlayerRef playerRef)
        {
            if (!playerRef.IsValid)
            {
                throw new Exception($"Failed waiting for invalid player({playerRef})'s data");
            }

            if (!pendingPlayers.Add(playerRef))
            {
                throw new Exception($"Failed waiting for player({playerRef})'s data");
            }

            StartCheckPendingPlayer();

            logger.LogInformation("Start waiting for player({PlayerRef})'s data for {Interval}", playerRef, PlayerDataWaitInterval);
        }

        private void StopWaitPlayerData(PlayerRef playerRef)
        {
            if (pendingPlayers.Remove(playerRef))
            {
                if (pendingPlayers.Count == 0)
                {
                    StopCheckPendingPlayer();
                }
            }
        }

        private void CheckExpiredPendingPlayer(long invokeCount)
        {
            // Fetch expired pending players
            var expiredPlayers = new HashSet<PlayerRef>();
            pendingPlayers.TrimExpiredItems(DateTime.UtcNow, expiredPlayers);

            // Disconnect with the expired pending players
            foreach (var expiredPlayer in expiredPlayers)
            {
                networkRunner.Disconnect(expiredPlayer);
                logger.LogWarning("Disconnect to player({PlayerRef}) because of player's data timeout({Interval})", expiredPlayer, PlayerDataWaitInterval);
            }

            // Stop the checking routine if there are no more pending players.
            if (pendingPlayers.Count == 0)
            {
                StopCheckPendingPlayer();
            }
        }

        private void StartCheckPendingPlayer()
        {
            if (routineDisposable == null)
            {
                var observable = Observable.Interval(ExpiredPlayerCheckInterval).ObserveOnMainThread();
                routineDisposable = UniRx.ObservableExtensions.Subscribe(observable, CheckExpiredPendingPlayer);
            }
        }

        private void StopCheckPendingPlayer()
        {
            if (routineDisposable != null)
            {
                routineDisposable.Dispose();
                routineDisposable = null;
            }
        }

        private void StopSubscribeMsg()
        {
            if (msgDisposable != null)
            {
                msgDisposable.Dispose();
                msgDisposable = null;
            }
        }
    }
}
