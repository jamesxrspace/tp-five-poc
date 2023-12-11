using UniRx;
using UnityEngine;

namespace TPFive.Game.Camera
{
    public interface ICamera : IHasAliveCheck
    {
        /// <summary>
        /// Gets the name of camera.
        /// </summary>
        /// <value>camera's name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the state of camera.<br/>
        /// If state is <see cref="CameraState.Live"/> will send the view content of this to live,
        /// otherwise not send.
        /// </summary>
        /// <value>camera current state.</value>
        IReactiveProperty<CameraState> State { get; }

        /// <summary>
        /// Gets the custom culling mask for camera.
        /// </summary>
        /// <value>custom culling mask. if NULL means not need to override.</value>
        IReadOnlyReactiveProperty<int?> CullingMaskOverride { get; }

        /// <summary>
        /// Gets or sets the camera's pose in world space.
        /// </summary>
        /// <value>camera current position and rotation.</value>
        Pose Pose { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow interact with move input.
        /// </summary>
        /// <value>If TRUE means allow interact move input, otherwise not.</value>
        bool AllowInteractWithMoveInput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow interact with rotate input.
        /// </summary>
        /// <value>If TRUE means allow interact rotate input, otherwise not.</value>
        bool AllowInteractWithRotateInput { get; set; }

        /// <summary>
        /// Set this camera's pose back to starting status immediately.
        /// </summary>
        void ResetToStartingPose();
    }
}
