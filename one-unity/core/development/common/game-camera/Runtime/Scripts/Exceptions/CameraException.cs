using System;
using System.Runtime.Serialization;

namespace TPFive.Game.Camera
{
    /// <summary>
    /// The exception will be thrown when use camera got some unexpected case.
    /// </summary>
    /// <example>
    /// <code>
    /// public void ExampleFunc(ICamera camera)
    /// {
    ///     if (camera is not ICameraLookAtTarget cameraLookAtTarget)
    ///     {
    ///         return;
    ///     }
    ///     if (cameraLookAtTarget.LookAtTarget.Value == null)
    ///     {
    ///         throw new CameraException("The lookat target is not found.");
    ///     }
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public class CameraException : Exception
    {
        public CameraException()
        {
        }

        public CameraException(string message)
            : base(message)
        {
        }

        public CameraException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CameraException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
