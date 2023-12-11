using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using UnityEngine.XR.Interaction.Toolkit;

namespace TPFive.Extended.InputXREvent
{
    public class XRInputData
    {
        public XRInputData()
        {
            Hand = Handedness.None;
            Handshape = HandshapeTypes.HandshapeId.None;
            Args = null;
        }

        public XRInputData(Handedness hand, HandshapeTypes.HandshapeId handshape, BaseInteractionEventArgs args)
        {
            Hand = hand;
            Handshape = handshape;
            Args = args;
        }

        public Handedness Hand { get; set; }

        public HandshapeTypes.HandshapeId Handshape { get; set; }

        public BaseInteractionEventArgs Args { get; set; }
    }
}
