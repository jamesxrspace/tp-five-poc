using System;
using UnityEngine.Events;

namespace TPFive.Extended.InputXREvent
{
    [Serializable]
    public class XRInputDataEvent : UnityEvent<XRInputData>
    {
        public enum EventType
        {
            /// <summary>
            /// Pointer down and up immediately.
            /// </summary>
            Click,

            /// <summary>
            /// Pointer down and up twice immediately.
            /// </summary>
            DoubleClick,

            /// <summary>
            /// Pointer down for a while.
            /// </summary>
            LongPress,
        }
    }
}
