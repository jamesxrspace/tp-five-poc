using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using VContainer;

namespace TPFive.Room
{
    public class NetSceneController : NetworkBehaviour, INetworkRunnerCallbacks
    {
        [Inject]
        private INetSceneLoader netSceneLoader;

        /// <summary>
        /// Gets or sets the addressable key of the content scene the room is going to host.
        /// </summary>
        /// <value>
        /// The 36-byte addressable key of the content scene the room is going to host.
        /// </value>
        public string TargetSceneKey { get; set; }

        /// <summary>
        /// Gets the addressable key of the content scene which has been loaded successfully on host/server in the room.
        /// </summary>
        /// <value>
        /// The 36-byte addressable key of the content scene which has been loaded successfully on host/server in the room.
        /// </value>
        [Networked(OnChanged = nameof(OnSceneKeyPublishedChanged), OnChangedTargets = OnChangedTargets.Proxies)]
        [Capacity(36)]
        public string SceneKeyPublished { get; private set; }

        /// <summary>
        /// Gets the addressable key of the content scene which has been loaded successfully locally on host/server/client in the room.
        /// </summary>
        /// <value>
        /// The 36-byte addressable key of the content scene which has been loaded successfully locally on host/server/client in the room.
        /// </value>
        public string SceneKeyLoaded { get; private set; }

        public override void Spawned()
        {
            Runner.AddCallbacks(this);
            if (Runner.IsServer)
            {
                netSceneLoader.LoadScene(TargetSceneKey);
            }
        }

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            SceneKeyLoaded = TargetSceneKey;

            if (Runner.IsServer)
            {
                SceneKeyPublished = TargetSceneKey;
            }
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        private static void OnSceneKeyPublishedChanged(Changed<NetSceneController> changed)
        {
            var behaviour = changed.Behaviour;
            if (behaviour.SceneKeyPublished == behaviour.SceneKeyLoaded)
            {
                return;
            }

            behaviour.netSceneLoader.LoadScene(behaviour.SceneKeyPublished);
        }
    }
}
