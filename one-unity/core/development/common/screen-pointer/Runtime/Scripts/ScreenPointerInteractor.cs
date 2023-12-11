using System.Collections.Generic;
using MixedReality.Toolkit;
using TPFive.Extended.InputSystem;
using TPFive.Game.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace TPFive.Extended.ScreenPointer
{
    public class ScreenPointerInteractor : XRRayInteractor, IRayInteractor
    {
        [SerializeField]
        private InputActionProperty pointerSelectAction;
        private bool _isPressed;

        /// <inheritdoc />
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            if (IsOverUI())
            {
                return false;
            }

            return base.CanSelect(interactable) && (!hasSelection || IsSelecting(interactable));
        }

        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            // When selection is active, force valid targets to be the current selection. This is done to ensure that selected objects remained hovered.
            if (hasSelection && isActiveAndEnabled)
            {
                targets.Clear();
                targets.AddRange(interactablesSelected);
            }
            else
            {
                base.GetValidTargets(targets);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (pointerSelectAction.action != null)
            {
                pointerSelectAction.action.started += OnPointerDown;
                pointerSelectAction.action.canceled += OnPointerUp;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (pointerSelectAction.action != null)
            {
                pointerSelectAction.action.started -= OnPointerDown;
                pointerSelectAction.action.canceled -= OnPointerUp;
            }
        }

        protected void FixedUpdate()
        {
            if (!_isPressed || !hasSelection)
            {
                return;
            }

            SetRayOrigin();
            var position = rayOriginTransform.position;
            var distanceToAttachTransform = Vector3.Distance(position, attachTransform.position);
            attachTransform.position = position + (rayOriginTransform.forward * distanceToAttachTransform);
        }

        private void OnPointerDown(InputAction.CallbackContext context)
        {
            if (IsOverUI())
            {
                return;
            }

            SetRayOrigin();
            _isPressed = true;
        }

        private void OnPointerUp(InputAction.CallbackContext context)
        {
            _isPressed = false;
        }

        private bool IsOverUI()
        {
            var pos = pointerSelectAction.action.ReadValue<Vector2>();
            return EventSystem.current.IsPointerOverUI(pos);
        }

        private void SetRayOrigin()
        {
            var pos = pointerSelectAction.action.ReadValue<Vector2>();
            var ray = CameraCache.Main.ScreenPointToRay(pos);
            rayOriginTransform.position = ray.origin;
            rayOriginTransform.forward = ray.direction;
#if  UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction, Color.yellow, 10f, true);
#endif
        }
    }
}