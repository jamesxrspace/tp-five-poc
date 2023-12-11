using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine.XR;

namespace TPFive.Extended.InputXREvent
{
    public class HandConstraintHold : HandConstraint
    {
        protected override bool IsValidController(XRNode? hand)
        {
            return base.IsValidController(hand) && HandshapeHelpers.IsHolding(hand.Value);
        }
    }
}
