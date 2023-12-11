using MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.XR;
using HandshapeId = MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace TPFive.Extended.InputXREvent
{
    public static class HandshapeHelpers
    {
        public static ITransmitterVR TransmitterVR { get; set; }

        public static bool TryGetHandshapeId(XRNode hand, out HandshapeId handshapeId)
        {
            handshapeId = HandshapeId.None;
            if (!hand.IsHand())
            {
                return false;
            }

            if (XRSubsystemHelpers.HandsAggregator == null)
            {
                return false;
            }

            if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, hand, out HandJointPose palmPose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.ThumbTip, hand, out HandJointPose thumbTipPose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip, hand, out HandJointPose indexTipPose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexDistal, hand, out HandJointPose indexDistalPose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexIntermediate, hand, out HandJointPose indexIntermediatePose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexProximal, hand, out HandJointPose indexProximalPose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.MiddleTip, hand, out HandJointPose middleTipPose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.RingTip, hand, out HandJointPose ringTipPose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.LittleTip, hand, out HandJointPose littleTipPose))
            {
                var indexTipDotMiddleTip = Vector3.Dot(indexTipPose.Forward, middleTipPose.Forward);
                var indexTipDotRingTip = Vector3.Dot(indexTipPose.Forward, ringTipPose.Forward);
                var indexTipDotLittleTip = Vector3.Dot(indexTipPose.Forward, littleTipPose.Forward);

                if (indexTipDotMiddleTip > 0 &&
                    indexTipDotRingTip > 0 &&
                    indexTipDotLittleTip > 0)
                {
                    if (Vector3.Dot(palmPose.Forward, indexTipPose.Forward) > 0)
                    {
                        handshapeId = HandshapeId.Flat;
                    }
                    else
                    {
                        handshapeId = (Vector3.Dot(Vector3.up, palmPose.Up) < 0) ? HandshapeId.TeleportEnd : HandshapeId.Grab;
                    }
                }
                else if (indexTipDotMiddleTip < 0 &&
                         indexTipDotRingTip < 0 &&
                         indexTipDotLittleTip < 0)
                {
                    if (Vector3.Dot(Vector3.up, palmPose.Up) > 0)
                    {
                        float indexFingerLength = Vector3.Distance(indexTipPose.Position, indexDistalPose.Position) +
                                                  Vector3.Distance(indexDistalPose.Position, indexIntermediatePose.Position) +
                                                  Vector3.Distance(indexIntermediatePose.Position, indexProximalPose.Position);
                        float pinchDistance = Vector3.Distance(indexTipPose.Position, thumbTipPose.Position);

                        handshapeId = (pinchDistance < indexFingerLength) ? HandshapeId.Pinch : HandshapeId.Open;
                    }
                    else
                    {
                        handshapeId = HandshapeId.TeleportStart;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool IsHolding(XRNode hand)
        {
            if (!hand.IsHand())
            {
                return false;
            }

            if (XRSubsystemHelpers.HandsAggregator == null)
            {
                return false;
            }

            if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.ThumbTip, hand, out HandJointPose thumbTipPose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip, hand, out HandJointPose indexTipPose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexDistal, hand, out HandJointPose indexDistalPose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexIntermediate, hand, out HandJointPose indexIntermediatePose) &&
                XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexProximal, hand, out HandJointPose indexProximalPose))
            {
                var indexFingerLength = Vector3.Distance(indexTipPose.Position, indexDistalPose.Position) +
                                        Vector3.Distance(indexDistalPose.Position, indexIntermediatePose.Position) +
                                        Vector3.Distance(indexIntermediatePose.Position, indexProximalPose.Position);
                var pinchDistance = Vector3.Distance(indexTipPose.Position, thumbTipPose.Position);
                return pinchDistance < indexFingerLength;
            }

            return false;
        }
    }
}