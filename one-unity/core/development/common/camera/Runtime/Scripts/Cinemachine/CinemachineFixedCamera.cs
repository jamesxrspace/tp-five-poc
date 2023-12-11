namespace TPFive.Extended.Camera.Cinemachine
{
    /// <summary>
    /// Will not moving or rotating by itself.
    /// </summary>
    public sealed class CinemachineFixedCamera : CinemachineCameraBase
    {
        public override bool AllowInteractWithMoveInput
        {
            get => false;
            set { /* Do nothing */ }
        }

        public override bool AllowInteractWithRotateInput
        {
            get => false;
            set { /* Do nothing */ }
        }
    }
}
