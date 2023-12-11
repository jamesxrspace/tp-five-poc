using UnityEngine.InputSystem;

namespace TPFive.Game.Input
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        /// <summary>
        /// Register input action asset into service.
        /// </summary>
        /// <param name="inputActionAsset"></param>
        void RegisterInputActionAsset(InputActionAsset inputActionAsset);

        /// <summary>
        /// Unregister input action asset from service.
        /// </summary>
        /// <param name="inputActionAsset"></param>
        void UnregisterInputActionAsset(InputActionAsset inputActionAsset);

        /// <summary>
        /// Try get input action from service.
        /// </summary>
        /// <param name="actionNameOrId">Name of the action as either a "map/action" combination (e.g. "gameplay/fire") or
        /// a simple name. In the former case, the name is split at the '/' slash and the first part is used to find
        /// a map with that name and the second part is used to find an action with that name inside the map. In the
        /// latter case, all maps are searched in order and the first action that has the given name in any of the maps
        /// is returned. Note that name comparisons are case-insensitive.
        ///
        /// Alternatively, the given string can be a GUID as given by <see cref="InputAction.id"/>.</param>
        /// <param name="inputAction">A named input signal that can flexibly decide which input data to tap.</param>
        /// <returns>If TRUE means found, otherwise not.</returns>
        bool TryGetInputAction(string actionNameOrId, out InputAction inputAction);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        /// <summary>
        /// Register input action asset into service.
        /// </summary>
        /// <param name="inputActionAsset"></param>
        void RegisterInputActionAsset(InputActionAsset inputActionAsset);

        /// <summary>
        /// Unregister input action asset from service.
        /// </summary>
        /// <param name="inputActionAsset"></param>
        void UnregisterInputActionAsset(InputActionAsset inputActionAsset);

        /// <summary>
        /// Try get input action from service.
        /// </summary>
        /// <param name="actionNameOrId">Name of the action as either a "map/action" combination (e.g. "gameplay/fire") or
        /// a simple name. In the former case, the name is split at the '/' slash and the first part is used to find
        /// a map with that name and the second part is used to find an action with that name inside the map. In the
        /// latter case, all maps are searched in order and the first action that has the given name in any of the maps
        /// is returned. Note that name comparisons are case-insensitive.
        ///
        /// Alternatively, the given string can be a GUID as given by <see cref="InputAction.id"/>.</param>
        /// <param name="inputAction">A named input signal that can flexibly decide which input data to tap.</param>
        /// <returns>If TRUE means found, otherwise not.</returns>
        bool TryGetInputAction(string actionNameOrId, out InputAction inputAction);
    }
}
