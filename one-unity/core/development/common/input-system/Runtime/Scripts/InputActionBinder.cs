using System;
using Microsoft.Extensions.Logging;
using TPFive.Extended.InputSystem.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using IInputService = TPFive.Game.Input.IService;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.InputSystem
{
    /// <summary>
    /// Redirect to real input action and bind/unbind input events.
    /// </summary>
    public sealed class InputActionBinder : MonoBehaviour
    {
        [SerializeField]
        private InputActionProperty inputAction;

        private ILogger log;
        private InputAction realInputAction;

        private Action<InputAction.CallbackContext> onStartedCallback;
        private Action<InputAction.CallbackContext> onPerformedCallback;
        private Action<InputAction.CallbackContext> onCanceledCallback;

        private bool isBind = true;

        public bool IsBind
        {
            get => isBind;
            set
            {
                if (isBind == value)
                {
                    return;
                }

                isBind = value;
                RebindInputEventsIfNeed();
            }
        }

        [Inject]
        public void Construct(ILoggerFactory loggerFactory, IInputService inputService)
        {
            this.log = loggerFactory.CreateLogger<InputActionBinder>();

            // make sure used original input action instance
            // not the one in addressable cloned.
            if (!inputService.TryGetInputAction(inputAction.action.id.ToString(), out realInputAction))
            {
                this.log.LogError(
                    "{Method}: Cannot find input action '{ActionId}'",
                    nameof(Construct),
                    inputAction.action.id.ToString());

                return;
            }

            RebindInputEventsIfNeed();
        }

        public void SetupCallbacks(
            Action<InputAction.CallbackContext> onStarted,
            Action<InputAction.CallbackContext> onPerformed,
            Action<InputAction.CallbackContext> onCanceled)
        {
            onStartedCallback = onStarted;
            onPerformedCallback = onPerformed;
            onCanceledCallback = onCanceled;
        }

        private void RebindInputEventsIfNeed()
        {
            if (realInputAction == null)
            {
                return;
            }

            realInputAction.UnbindEvents(OnActionStarted, OnActionPerformed, OnActionCanceled);

            if (!isBind)
            {
                log.LogDebug("{Method}: Unbind events", nameof(RebindInputEventsIfNeed));
                return;
            }

            log.LogDebug("{Method}: Bind events", nameof(RebindInputEventsIfNeed));
            realInputAction.BindEvents(OnActionStarted, OnActionPerformed, OnActionCanceled);
        }

        private void OnActionStarted(InputAction.CallbackContext context)
        {
            onStartedCallback?.Invoke(context);
        }

        private void OnActionPerformed(InputAction.CallbackContext context)
        {
            onPerformedCallback?.Invoke(context);
        }

        private void OnActionCanceled(InputAction.CallbackContext context)
        {
            onCanceledCallback?.Invoke(context);
        }
    }
}
