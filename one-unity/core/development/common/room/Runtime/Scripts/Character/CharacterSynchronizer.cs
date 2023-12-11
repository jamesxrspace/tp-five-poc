using EasyCharacterMovement;
using Fusion;
using Microsoft.Extensions.Logging;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Room
{
    [RequireComponent(typeof(CharacterMovement), typeof(Character))]
    public class CharacterSynchronizer : NetworkBehaviour, IBeforeAllTicks, IAfterAllTicks
    {
        [SerializeField]
        private CharacterMovement characterMovement;

        [SerializeField]
        private Character character;

        private ILogger logger;

        [Networked]
        private ref CharacterNetState NetState => ref MakeRef<CharacterNetState>();

        [Inject]
        public void Construct(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<CharacterSynchronizer>();
        }

        public override void FixedUpdateNetwork()
        {
            if (character != null)
            {
                character.Simulate(Runner.DeltaTime);
            }
        }

        void IBeforeAllTicks.BeforeAllTicks(bool resimulation, int tickCount)
        {
            if (HasStateAuthority)
            {
                return;
            }

            if (!resimulation)
            {
                return;
            }

            // Synchronize CharacterMovement's state with the State Authority
            if (characterMovement != null)
            {
                character.SetMovementDirection(NetState.MovementDirection);
                characterMovement.SetState(
                    new CharacterMovement.State(
                    NetState.Position,
                    NetState.Rotation,
                    NetState.Velocity,
                    NetState.IsConstrainedToGround,
                    NetState.UnconstrainedTimer,
                    NetState.HitGround,
                    NetState.IsWalkable,
                    NetState.GroundNormal));
            }

            // Synchronize Character's state with the State Authority
            if (character != null)
            {
                character.SetMovementMode(NetState.MovementMode);
                character.SetRotationMode(NetState.RotationMode);
                if (NetState.IsCrouching)
                {
                    character.Crouch();
                }
                else
                {
                    character.StopCrouching();
                }
            }
        }

        void IAfterAllTicks.AfterAllTicks(bool resimulation, int tickCount)
        {
            if (!HasStateAuthority)
            {
                return;
            }

            // Fetch character net state
            var characterMovementState = characterMovement.GetState();
            NetState.MovementDirection = character.GetMovementDirection();
            NetState.Position = characterMovementState.position;
            NetState.Rotation = characterMovementState.rotation;
            NetState.Velocity = characterMovementState.velocity;
            NetState.IsConstrainedToGround = characterMovementState.isConstrainedToGround;
            NetState.UnconstrainedTimer = characterMovementState.unconstrainedTimer;
            NetState.HitGround = characterMovementState.hitGround;
            NetState.IsWalkable = characterMovementState.isWalkable;
            NetState.GroundNormal = characterMovementState.groundNormal;
            NetState.MovementMode = character.GetMovementMode();
            NetState.RotationMode = character.GetRotationMode();
            NetState.IsCrouching = character.IsCrouching();
        }

        protected void Awake()
        {
            if (character == null)
            {
                logger.LogError("Initialize failed: missing component of character!");
                return;
            }

            // Disable the simulation of Character and it is simulated in FixedUpdateNetwork().
            character.enableLateFixedUpdate = false;
        }

        private struct CharacterNetState : INetworkStruct
        {
            public Vector3 MovementDirection;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public NetworkBool IsConstrainedToGround;
            public float UnconstrainedTimer;
            public NetworkBool HitGround;
            public NetworkBool IsWalkable;
            public Vector3 GroundNormal;
            public MovementMode MovementMode;
            public RotationMode RotationMode;
            public NetworkBool IsCrouching;
        }
    }
}
