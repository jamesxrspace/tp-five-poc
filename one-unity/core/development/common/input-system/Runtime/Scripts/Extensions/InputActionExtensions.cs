using System;
using UnityEngine.InputSystem;

namespace TPFive.Extended.InputSystem.Extensions
{
    public static class InputActionExtensions
    {
        public static void EnableIfUseIsolated(this InputActionProperty inputActionProperty, string actionName)
        {
            var action = inputActionProperty.action;
            if (action == null || inputActionProperty.reference != null)
            {
                return;
            }

            action.Rename($"[Isolated] {actionName}");
            action.Enable();
        }

        public static void DisableIfUseIsolated(this InputActionProperty inputActionProperty)
        {
            var action = inputActionProperty.action;
            if (action == null || inputActionProperty.reference != null)
            {
                return;
            }

            action.Disable();
        }

        public static void BindEvents(
            this InputActionProperty inputActionProperty,
            Action<InputAction.CallbackContext> onStarted,
            Action<InputAction.CallbackContext> onPerformed,
            Action<InputAction.CallbackContext> onCanceled)
        {
            var action = inputActionProperty.action;
            if (action == null)
            {
                return;
            }

            if (onStarted != null)
            {
                action.started += onStarted;
            }

            if (onPerformed != null)
            {
                action.performed += onPerformed;
            }

            if (onCanceled != null)
            {
                action.canceled += onCanceled;
            }
        }

        public static void BindEvents(
            this InputAction action,
            Action<InputAction.CallbackContext> onStarted,
            Action<InputAction.CallbackContext> onPerformed,
            Action<InputAction.CallbackContext> onCanceled)
        {
            if (onStarted != null)
            {
                action.started += onStarted;
            }

            if (onPerformed != null)
            {
                action.performed += onPerformed;
            }

            if (onCanceled != null)
            {
                action.canceled += onCanceled;
            }
        }

        public static void UnbindEvents(
            this InputActionProperty inputActionProperty,
            Action<InputAction.CallbackContext> onStarted,
            Action<InputAction.CallbackContext> onPerformed,
            Action<InputAction.CallbackContext> onCanceled)
        {
            var action = inputActionProperty.action;
            if (action == null)
            {
                return;
            }

            if (onStarted != null)
            {
                action.started -= onStarted;
            }

            if (onPerformed != null)
            {
                action.performed -= onPerformed;
            }

            if (onCanceled != null)
            {
                action.canceled -= onCanceled;
            }
        }

        public static void UnbindEvents(
            this InputAction action,
            Action<InputAction.CallbackContext> onStarted,
            Action<InputAction.CallbackContext> onPerformed,
            Action<InputAction.CallbackContext> onCanceled)
        {
            if (onStarted != null)
            {
                action.started -= onStarted;
            }

            if (onPerformed != null)
            {
                action.performed -= onPerformed;
            }

            if (onCanceled != null)
            {
                action.canceled -= onCanceled;
            }
        }
    }
}
