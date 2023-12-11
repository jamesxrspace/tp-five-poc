using UniRx;
using UnityEngine;

namespace TPFive.Extended.InputDeviceProvider.OnScreen
{
    /// <summary>
    /// Controller of on-screen stick.
    /// </summary>
    public interface IOnScreenStickController
    {
        /// <summary>
        /// Gets the active state of the controller.
        /// <br/><br/>
        /// Can use <see cref="UniRx.ObservableExtensions.Subscribe"/> method to subscribe the active state.
        /// <br/>
        /// Allow user set boolean value into <see cref="IsActive.Value"/> to control the active state.
        /// </summary>
        /// <value>
        /// If TRUE means controller is active, otherwise is inactive.
        /// </value>
        IReactiveProperty<bool> IsActive { get; }

        /// <summary>
        /// Sets up the interact area of the controller.
        /// </summary>
        /// <param name="anchorMin">minimum anchor is the bottom left corner of the screen.</param>
        /// <param name="anchorMax">max anchor is the top right corner of the screen.</param>
        void SetupInteractArea(Vector2 anchorMin, Vector2 anchorMax);
    }
}