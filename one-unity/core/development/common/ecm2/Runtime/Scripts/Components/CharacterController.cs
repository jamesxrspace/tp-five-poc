using EasyCharacterMovement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TPFive.Extended.ECM2
{
    public class CharacterController : MonoBehaviour
    {
        //
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        public Character _character;

        private Vector2 _movementInput;

        public void OnMovement(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                _character.Jump();
            else if (context.canceled)
                _character.StopJumping();
        }

        //
        private void HandleInput()
        {
            // Add movement input relative to camera's view direction (in world space)

            Vector3 movementDirection = Vector3.zero;

            movementDirection += Vector3.right * _movementInput.x;
            movementDirection += Vector3.forward * _movementInput.y;

            movementDirection = movementDirection.relativeTo(_camera.transform);

            _character.SetMovementDirection(movementDirection);
        }

        //
        private void Update()
        {
            HandleInput();
        }
    }
}
