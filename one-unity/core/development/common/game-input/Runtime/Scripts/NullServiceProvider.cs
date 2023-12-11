using UnityEngine.InputSystem;

namespace TPFive.Game.Input
{
    /// <summary>
    /// This is the default service provider for related service.
    /// <br/><br/>
    /// Create this class by manual instead of codegen(<see cref="TPFive.SCG.ServiceEco.Abstractions.ServiceProvidedByAttribute"/>)
    /// cause the codegen can't handle <see cref="InputActionAsset"/> type.
    /// </summary>
    public class NullServiceProvider : TPFive.Game.Input.IServiceProvider
    {
        /// <summary>
        /// Action with multiple parameters.
        /// </summary>
        private readonly T1MultiObjParamDelegate<string> _actionHandler;

        public NullServiceProvider(
            T1MultiObjParamDelegate<string> actionHandler)
        {
            _actionHandler = actionHandler;
        }
        public void RegisterInputActionAsset(InputActionAsset inputActionAsset)
        {
            _actionHandler?.Invoke("{MethodName} - {InputActionAsset}", nameof(RegisterInputActionAsset), inputActionAsset);
        }

        public void UnregisterInputActionAsset(InputActionAsset inputActionAsset)
        {
            _actionHandler?.Invoke("{MethodName} - {InputActionAsset}", nameof(UnregisterInputActionAsset), inputActionAsset);
        }

        public bool TryGetInputAction(string actionNameOrId, out InputAction inputAction)
        {
            inputAction = default;
            _actionHandler?.Invoke("{MethodName} - {ActionNameOrId} - {InputAction}", nameof(TryGetInputAction), actionNameOrId, inputAction);
            return default;
        }
    }
}
