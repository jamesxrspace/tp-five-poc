using System;
using System.Collections.Generic;
using EasyCharacterMovement;
using Fusion;
using Fusion.Sockets;
using TPFive.Game.Avatar;
using TPFive.Game.Utils;
using UnityEngine;

namespace TPFive.Room
{
    public struct PlayerNetInput : INetworkInput
    {
        public const int ButtonIndexJump = 1;
        public const int ButtonIndexCrouch = 2;

        public Vector3 MovementDir;
        public NetworkButtons ActionButtons;

        public void SetButton(int index, bool state)
        {
            ActionButtons.Set(index, state);
        }
    }

    [OrderAfter(typeof(CharacterSynchronizer))]
    public class PlayerInputHandler : NetworkBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField]
        private PlayerInputTransfer playerInputTransfer;

        private Transform mainCamera;
        private Character character;
        private PlayerNetInput playerNetInput = default;
        private bool jumpPressed;
        private bool crouchPressed;

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput inputHolder)
        {
            // Gather Fusion input
            inputHolder.Set(playerNetInput);
        }

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public override void Spawned()
        {
            if (playerInputTransfer == null)
            {
                return;
            }

            if (playerInputTransfer.enabled != HasInputAuthority)
            {
                playerInputTransfer.enabled = HasInputAuthority;
            }

            if (!HasInputAuthority)
            {
                return;
            }

            playerInputTransfer.OnMoveStarted.AddListener(OnMoveStarted);
            playerInputTransfer.OnMovePerformed.AddListener(OnMovePerformed);
            playerInputTransfer.OnMoveCanceled.AddListener(OnMoveCanceled);
            playerInputTransfer.OnJumpStarted.AddListener(OnJumpStarted);
            playerInputTransfer.OnJumpCanceled.AddListener(OnJumpCanceled);
            playerInputTransfer.OnCrouchStarted.AddListener(OnCrouchStarted);
            playerInputTransfer.OnCrouchCanceled.AddListener(OnCrouchCanceled);

            Runner.AddCallbacks(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (playerInputTransfer == null
                || !HasInputAuthority)
            {
                return;
            }

            playerInputTransfer.OnMoveStarted.RemoveListener(OnMoveStarted);
            playerInputTransfer.OnMovePerformed.RemoveListener(OnMovePerformed);
            playerInputTransfer.OnMoveCanceled.RemoveListener(OnMoveCanceled);
            playerInputTransfer.OnJumpStarted.RemoveListener(OnJumpStarted);
            playerInputTransfer.OnJumpCanceled.RemoveListener(OnJumpCanceled);
            playerInputTransfer.OnCrouchStarted.RemoveListener(OnCrouchStarted);
            playerInputTransfer.OnCrouchCanceled.RemoveListener(OnCrouchCanceled);

            runner.RemoveCallbacks(this);
        }

        public override void FixedUpdateNetwork()
        {
            // Apply Fusion input
            if (GetInput(out PlayerNetInput tickInput))
            {
                if (character != null)
                {
                    character.SetMovementDirection(tickInput.MovementDir);

                    if (tickInput.ActionButtons.IsSet(PlayerNetInput.ButtonIndexJump))
                    {
                        character.Jump();
                    }
                    else
                    {
                        character.StopJumping();
                    }

                    if (tickInput.ActionButtons.IsSet(PlayerNetInput.ButtonIndexCrouch))
                    {
                        character.Crouch();
                    }
                    else
                    {
                        character.StopCrouching();
                    }
                }
            }
        }

        protected void Awake()
        {
            CacheComponents();
        }

        private void CacheComponents()
        {
            if (CameraCache.Main != null)
            {
                mainCamera = CameraCache.Main.transform;
            }

            character = GetComponent<Character>();
        }

        private void OnMoveStarted(Vector2 moveInput)
        {
            OnMove(moveInput);
        }

        private void OnMovePerformed(Vector2 moveInput)
        {
            OnMove(moveInput);
        }

        private void OnMoveCanceled()
        {
            OnMove(Vector2.zero);
        }

        private void OnMove(Vector2 moveInput)
        {
            // Add movement input in world space
            Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

            // If Camera is assigned, add input movement relative to camera look direction
            if (mainCamera != null)
            {
                moveDir = (moveDir.x * mainCamera.right) + (moveDir.z * mainCamera.forward);
                moveDir.y = 0f;
            }

            playerNetInput.MovementDir = moveDir;
        }

        private void OnJumpStarted()
        {
            OnJump(true);
        }

        private void OnJumpCanceled()
        {
            OnJump(false);
        }

        private void OnJump(bool jumpInput)
        {
            playerNetInput.SetButton(PlayerNetInput.ButtonIndexJump, jumpInput);
        }

        private void OnCrouchStarted()
        {
            OnCrouch(true);
        }

        private void OnCrouchCanceled()
        {
            OnCrouch(false);
        }

        private void OnCrouch(bool crouchInput)
        {
            playerNetInput.SetButton(PlayerNetInput.ButtonIndexCrouch, crouchInput);
        }
    }
}
