using System;
using EasyCharacterMovement;
using UnityEngine;

namespace TPFive.Game.Avatar
{
    public class ECMAnimatorCommunicator
    {
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Turn = Animator.StringToHash("Turn");
        private static readonly int Ground = Animator.StringToHash("OnGround");
        private static readonly int Crouch = Animator.StringToHash("Crouch");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int JumpLeg = Animator.StringToHash("JumpLeg");

        private readonly Character _character;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly float _runningCycleOffset;
        private bool _isLocomotionStarted;

        public ECMAnimatorCommunicator(Character character, Transform transform, Animator animator, float runningCycleOffset)
        {
            this._character = character;
            this._transform = transform;
            this._animator = animator;
            this._runningCycleOffset = runningCycleOffset;
        }

        public event Action LocomotionStarted;

        public bool CanRun { get; set; } = true;

        public void Update()
        {
            float deltaTime = Time.deltaTime;
            bool newLocomotionState;

            // Collects character's states
            var movementDirection = _character.GetMovementDirection();
            var maxSpeed = _character.GetMaxSpeed();
            var speed = _character.GetSpeed();
            var isGrounded = _character.IsGrounded();
            var isCrouching = _character.IsCrouching();
            var isJumping = _character.IsJumping();
            var isFalling = _character.IsFalling();
            var velocity = _character.GetVelocity();

            // Compute input move vector in local space
            Vector3 move = _transform.InverseTransformDirection(movementDirection);

            float forwardAmount = _character.useRootMotion && _character.GetRootMotionController()
                ? move.z
                : Mathf.InverseLerp(0.0f, maxSpeed, speed);

            if (!CanRun)
            {
                forwardAmount = Mathf.Clamp(forwardAmount, 0.0f, AvatarConfig.MaxWalkVelocity);
            }

            // Update the animator parameters
            _animator.SetFloat(Forward, forwardAmount, 0.1f, deltaTime);
            _animator.SetFloat(Turn, Mathf.Atan2(move.x, move.z), 0.1f, deltaTime);

            _animator.SetBool(Ground, isGrounded);
            _animator.SetBool(Crouch, isCrouching);

            if (isFalling)
            {
                _animator.SetFloat(Jump, velocity.y, 0.1f, deltaTime);
            }

             /* Calculate which leg is behind, so as to leave that leg trailing in the jump animation
             (This code is reliant on the specific run cycle offset in our animations,
             and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5) */
            float runCycle = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + _runningCycleOffset, 1.0f);
            float jumpLeg = (runCycle < 0.5f ? 1.0f : -1.0f) * forwardAmount;

            if (isGrounded)
            {
                _animator.SetFloat(JumpLeg, jumpLeg);
            }

            newLocomotionState = speed > 0f || isJumping || isCrouching;

            if (newLocomotionState != _isLocomotionStarted && newLocomotionState)
            {
                LocomotionStarted?.Invoke();
            }

            _isLocomotionStarted = newLocomotionState;
        }
    }
}