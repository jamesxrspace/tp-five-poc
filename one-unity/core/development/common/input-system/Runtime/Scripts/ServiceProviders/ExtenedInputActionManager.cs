using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using IInputService = TPFive.Game.Input.IService;

namespace TPFive.Extended.InputSystem
{
    /// <summary>
    /// 1. Use this class to automatically enable or disable all the inputs of type <see cref="InputAction"/>
    /// in a list of assets of type <see cref="InputActionAsset"/>.<br/>
    /// 2. Register or Unregister all the inputs into <see cref="TPFive.Game.Input.IService"/>.
    /// <br/>
    /// this class is base on <see cref="UnityEngine.XR.Interaction.Toolkit.Inputs.InputActionManager"/> to extend.
    /// </summary>
    /// <remarks>
    /// Actions are initially disabled, meaning they do not listen/react to input yet.Use this
    /// class to mass enable actions so that they actively listen for input and run callbacks.
    /// </remarks>
    /// <seealso cref="InputAction"/>
    public sealed class ExtenedInputActionManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Input action assets to affect when inputs are enabled or disabled.")]
        private List<InputActionAsset> actionAssets;

        private IInputService inputService;
        private bool isRegistered;

        [Inject]
        public void Construct(IInputService inputService)
        {
            this.inputService = inputService;

            if (!this.enabled || isRegistered)
            {
                return;
            }

            RegisterInput();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnEnable()
        {
            EnableInput();

            if (inputService == null || isRegistered)
            {
                return;
            }

            RegisterInput();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnDisable()
        {
            DisableInput();

            if (inputService == null || !isRegistered)
            {
                return;
            }

            UnregisterInput();
        }

        /// <summary>
        /// Enable all actions referenced by this component.
        /// </summary>
        /// <remarks>
        /// Unity will automatically call this function when this <see cref="InputActionManager"/> component is enabled.
        /// However, this method can be called to enable input manually, such as after disabling it with <see cref="DisableInput"/>.
        /// <br />
        /// Enabling inputs only enables the action maps contained within the referenced
        /// action map assets (see <see cref="actionAssets"/>).
        /// </remarks>
        /// <seealso cref="DisableInput"/>
        private void EnableInput()
        {
            if (actionAssets == null)
            {
                return;
            }

            foreach (var actionAsset in actionAssets)
            {
                if (actionAsset == null)
                {
                    continue;
                }

                actionAsset.Enable();
            }
        }

        /// <summary>
        /// Disable all actions referenced by this component.
        /// </summary>
        /// <remarks>
        /// This function will automatically be called when this <see cref="InputActionManager"/> component is disabled.
        /// However, this method can be called to disable input manually, such as after enabling it with <see cref="EnableInput"/>.
        /// <br />
        /// Disabling inputs only disables the action maps contained within the referenced
        /// action map assets (see <see cref="actionAssets"/>).
        /// </remarks>
        /// <seealso cref="EnableInput"/>
        private void DisableInput()
        {
            if (actionAssets == null)
            {
                return;
            }

            foreach (var actionAsset in actionAssets)
            {
                if (actionAsset == null)
                {
                    continue;
                }

                actionAsset.Disable();
            }
        }

        /// <summary>
        /// Register input action asset into input service.
        /// </summary>
        private void RegisterInput()
        {
            isRegistered = true;

            if (actionAssets == null)
            {
                return;
            }

            foreach (var actionAsset in actionAssets)
            {
                if (actionAsset == null)
                {
                    continue;
                }

                inputService.RegisterInputActionAsset(actionAsset);
            }
        }

        /// <summary>
        /// Unregister input action asset from input service.
        /// </summary>
        private void UnregisterInput()
        {
            isRegistered = false;

            if (actionAssets == null)
            {
                return;
            }

            foreach (var actionAsset in actionAssets)
            {
                if (actionAsset == null)
                {
                    continue;
                }

                inputService.UnregisterInputActionAsset(actionAsset);
            }
        }
    }
}
