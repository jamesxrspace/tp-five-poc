using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace TPFive.Extended.InputXREvent
{
    public interface IXRInputEventReceiver
    {
    }

    public interface IXRClickEventReceiver : IXRInputEventReceiver
    {
        void OnClick(XRInputData inputData);
    }

    public interface IXRDoubleClickEventReceiver : IXRInputEventReceiver
    {
        void OnDoubleClick(XRInputData inputData);
    }

    public interface IXRLongPressEventReciver : IXRInputEventReceiver
    {
        void OnLongPress(XRInputData inputData);
    }

    public interface IXRCustomEventReceiver :
        IXRClickEventReceiver,
        IXRDoubleClickEventReceiver,
        IXRLongPressEventReciver
    {
    }

    public interface IXRMrtkEventReceiver
    {
        void OnHeadGazeHoverEntered(HoverEnterEventArgs hoverEnterEvent);

        void OnHeadGazeHoverExited(HoverExitEventArgs hoverExitEvent);

        void OnHeadGazeSelectEntered(SelectEnterEventArgs selectEnterEvent);

        void OnHeadGazeSelectExited(SelectExitEventArgs selectExitEvent);

        void OnHeadGazeUIHoverEntered(UIHoverEventArgs uIHoverEvent);

        void OnHeadGazeUIHoverExited(UIHoverEventArgs uIHoverEvent);

        void OnPokeHoverEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverEnterEventArgs hoverEnterEventArgs);

        void OnPokeHoverExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverExitEventArgs hoverExitEventArgs);

        void OnPokeSelectEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectEnterEventArgs selectEnterEventArgs);

        void OnPokeSelectExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectExitEventArgs selectExitEventArgs);

        void OnRayHoverEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverEnterEventArgs hoverEnterEventArgs);

        void OnRayHoverExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverExitEventArgs hoverExitEventArgs);

        void OnRaySelectEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectEnterEventArgs selectEnterEventArgs);

        void OnRaySelectExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectExitEventArgs selectExitEventArgs);

        void OnRayUIHoverEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            UIHoverEventArgs uIHoverEventArgs);

        void OnRayUIHoverExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            UIHoverEventArgs uIHoverEventArgs);

        void OnGrabHoverEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverEnterEventArgs hoverEnterEventArgs);

        void OnGrabHoverExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverExitEventArgs hoverExitEventArgs);

        void OnGrabSelectEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectEnterEventArgs selectEnterEventArgs);

        void OnGrabSelectExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectExitEventArgs selectExitEventArgs);

        void OnGazePinchHoverEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverEnterEventArgs hoverEnterEventArgs);

        void OnGazePinchHoverExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            HoverExitEventArgs hoverExitEventArgs);

        void OnGazePinchSelectEntered(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectEnterEventArgs selectEnterEventArgs);

        void OnGazePinchSelectExited(
            Handedness hand,
            HandshapeTypes.HandshapeId handshape,
            SelectExitEventArgs selectExitEventArgs);
    }

    public interface IXREventReceiver : IXRCustomEventReceiver, IXRMrtkEventReceiver
    {
    }
}