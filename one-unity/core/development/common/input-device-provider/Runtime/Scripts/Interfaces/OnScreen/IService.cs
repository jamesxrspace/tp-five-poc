using UniRx;

namespace TPFive.Extended.InputDeviceProvider.OnScreen
{
    /// <summary>
    /// Provides the on-screen controls.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Gets controller of the move stick.
        /// </summary>
        /// <value>
        /// 1. Check <see cref="MoveStickController.HasValue"/> before use to avoid null exception.<br/>
        /// 2. Get or Set move stick controller by <see cref="MoveStickController.Value"/> property.<br/>
        /// 3. Use <see cref="UniRx.ObservableExtensions.Subscribe"/> method to subscribe the controller change event.
        /// </value>
        IReactiveProperty<IOnScreenStickController> MoveStickController { get; }

        /// <summary>
        /// Gets controller of the rotate stick.
        /// </summary>
        /// <value>
        /// 1. Check <see cref="RotateStickController.HasValue"/> before use to avoid null exception.<br/>
        /// 2. Get or Set move stick controller by <see cref="RotateStickController.Value"/> property.<br/>
        /// 3. Use <see cref="UniRx.ObservableExtensions.Subscribe"/> method to subscribe the controller change event.
        /// </value>
        IReactiveProperty<IOnScreenStickController> RotateStickController { get; }
    }
}
