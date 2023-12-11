using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace TPFive.Room
{
    public sealed partial class FusionRoom : INetworkRunnerCallbacks
    {
        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (shutdownReason != ShutdownReason.Ok)
            {
                Logger.LogError("Fusion session has been shutdown({Reason}) abnormally", shutdownReason);

                // Release all the exisitng resource
                Release();

                // Start retrying to start Fusion session
                StartRetryStartGame();
            }
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
            if (runner.IsServer)
            {
                runner.AddCallbacks(CreatePlayerSpawner(runner));
                runner.Spawn(roomSetting.FusionTelemeterPrefab, Vector3.zero, Quaternion.identity);
                runner.Spawn(roomSetting.PlayerDataBrokerPrefab, Vector3.zero, Quaternion.identity);

                // Open the session to the public
                runner.SessionInfo.IsOpen = true;
            }

            // Notify that the room has been started(ready).
            OnStarted?.Invoke();
        }

        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}