using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Hands;
using Object = UnityEngine.Object;

namespace TPFive.Extended.Hands
{
    /// <summary>
    /// Presenting hand joints locomotion.
    /// </summary>
    /// <remarks>
    /// <br/><br/>
    /// Base on XR Hands package(v1.1.0) - Sample - HandVisualizer - HandVisualizer.cs - HandGameObjects class
    /// </remarks>
    public class HandGameObjects
    {
        private const float LineWidth = 0.005f;
        private static Vector3[] linePointsReuse = new Vector3[2];

        private GameObject handRoot;
        private GameObject drawJointsParent;

        private Transform[] jointXforms = new Transform[XRHandJointID.EndMarker.ToIndex()];
        private GameObject[] drawJoints = new GameObject[XRHandJointID.EndMarker.ToIndex()];
        private GameObject[] velocityParents = new GameObject[XRHandJointID.EndMarker.ToIndex()];
        private LineRenderer[] lines = new LineRenderer[XRHandJointID.EndMarker.ToIndex()];
        private bool isTracked;

        public HandGameObjects(
            Handedness handedness,
            Transform parent,
            GameObject meshPrefab,
            Material meshMaterial,
            GameObject debugDrawPrefab,
            GameObject velocityPrefab)
        {
            handRoot = Object.Instantiate(meshPrefab, parent);
            handRoot.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            Transform wristRootXform = null;
            for (int childIndex = 0; childIndex < handRoot.transform.childCount; ++childIndex)
            {
                var child = handRoot.transform.GetChild(childIndex);
                if (child.gameObject.name.EndsWith(XRHandJointID.Wrist.ToString()))
                {
                    wristRootXform = child;
                }
                else if (child.gameObject.name.EndsWith("Hand") && meshMaterial != null && child.TryGetComponent<SkinnedMeshRenderer>(out var renderer))
                {
                    renderer.sharedMaterial = meshMaterial;
                }
            }

            drawJointsParent = new GameObject();
            drawJointsParent.transform.SetParent(parent, false);
            drawJointsParent.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            drawJointsParent.name = handedness + " Hand Debug Draw Joints";

            if (wristRootXform == null)
            {
                Debug.LogWarning("Hand transform hierarchy not set correctly - couldn't find Wrist joint!");
                return;
            }

            AssignJoint(XRHandJointID.Wrist, wristRootXform, drawJointsParent.transform, debugDrawPrefab, velocityPrefab);
            for (int childIndex = 0; childIndex < wristRootXform.childCount; ++childIndex)
            {
                var child = wristRootXform.GetChild(childIndex);

                if (child.name.EndsWith(XRHandJointID.Palm.ToString()))
                {
                    AssignJoint(XRHandJointID.Palm, child, drawJointsParent.transform, debugDrawPrefab, velocityPrefab);
                    continue;
                }

                for (int fingerIndex = (int)XRHandFingerID.Thumb;
                     fingerIndex <= (int)XRHandFingerID.Little;
                     ++fingerIndex)
                {
                    var fingerId = (XRHandFingerID)fingerIndex;

                    var jointIdFront = fingerId.GetFrontJointID();
                    if (!child.name.EndsWith(jointIdFront.ToString()))
                    {
                        continue;
                    }

                    AssignJoint(jointIdFront, child, drawJointsParent.transform, debugDrawPrefab, velocityPrefab);
                    var lastChild = child;

                    int jointIndexBack = fingerId.GetBackJointID().ToIndex();
                    for (int jointIndex = jointIdFront.ToIndex() + 1;
                         jointIndex <= jointIndexBack;
                         ++jointIndex)
                    {
                        var jointId = XRHandJointIDUtility.FromIndex(jointIndex);
                        lastChild = FindFirstChildNameEndsWith(lastChild, jointId);
                        AssignJoint(jointId, lastChild, drawJointsParent.transform, debugDrawPrefab, velocityPrefab);
                    }
                }
            }
        }

        public void OnDestroy()
        {
            Object.Destroy(handRoot);
            handRoot = null;

            for (int jointIndex = 0; jointIndex < drawJoints.Length; ++jointIndex)
            {
                Object.Destroy(drawJoints[jointIndex]);
                drawJoints[jointIndex] = null;
            }

            for (int jointIndex = 0; jointIndex < velocityParents.Length; ++jointIndex)
            {
                Object.Destroy(velocityParents[jointIndex]);
                velocityParents[jointIndex] = null;
            }

            Object.Destroy(drawJointsParent);
            drawJointsParent = null;
        }

        public void ToggleDrawMesh(bool drawMesh)
        {
            for (int childIndex = 0; childIndex < handRoot.transform.childCount; ++childIndex)
            {
                var xform = handRoot.transform.GetChild(childIndex);
                if (xform.TryGetComponent<SkinnedMeshRenderer>(out var renderer))
                {
                    renderer.enabled = drawMesh;
                }
            }
        }

        public void ToggleDebugDrawJoints(bool debugDrawJoints)
        {
            for (int jointIndex = 0; jointIndex < drawJoints.Length; ++jointIndex)
            {
                ToggleRenderers<MeshRenderer>(debugDrawJoints, drawJoints[jointIndex].transform);
                lines[jointIndex].enabled = debugDrawJoints;
            }

            lines[0].enabled = false;
        }

        public void SetVelocityType(VelocityType velocityType)
        {
            for (int jointIndex = 0; jointIndex < velocityParents.Length; ++jointIndex)
            {
                ToggleRenderers<LineRenderer>(velocityType != VelocityType.None, velocityParents[jointIndex].transform);
            }
        }

        public void UpdateRootPose(XRHand hand)
        {
            var xform = jointXforms[XRHandJointID.Wrist.ToIndex()];
            xform.SetLocalPositionAndRotation(hand.rootPose.position, hand.rootPose.rotation);
        }

        public void UpdateJoints(
            XROrigin xrOrigin,
            XRHand hand,
            bool areJointsTracked,
            bool drawMeshes,
            bool debugDrawJoints,
            VelocityType velocityType)
        {
            if (isTracked != areJointsTracked)
            {
                ToggleDrawMesh(areJointsTracked && drawMeshes);
                ToggleDebugDrawJoints(areJointsTracked && debugDrawJoints);
                SetVelocityType(areJointsTracked ? velocityType : VelocityType.None);
                isTracked = areJointsTracked;
            }

            if (!isTracked)
            {
                return;
            }

            var originTransform = xrOrigin.Origin.transform;
            var originPose = new Pose(originTransform.position, originTransform.rotation);

            var wristPose = Pose.identity;
            UpdateJoint(debugDrawJoints, velocityType, originPose, hand.GetJoint(XRHandJointID.Wrist), ref wristPose);
            UpdateJoint(debugDrawJoints, velocityType, originPose, hand.GetJoint(XRHandJointID.Palm), ref wristPose, false);

            for (int fingerIndex = (int)XRHandFingerID.Thumb;
                fingerIndex <= (int)XRHandFingerID.Little;
                ++fingerIndex)
            {
                var parentPose = wristPose;
                var fingerId = (XRHandFingerID)fingerIndex;

                int jointIndexBack = fingerId.GetBackJointID().ToIndex();
                for (int jointIndex = fingerId.GetFrontJointID().ToIndex();
                    jointIndex <= jointIndexBack;
                    ++jointIndex)
                {
                    if (jointXforms[jointIndex] != null)
                    {
                        UpdateJoint(debugDrawJoints, velocityType, originPose, hand.GetJoint(XRHandJointIDUtility.FromIndex(jointIndex)), ref parentPose);
                    }
                }
            }
        }

        private static Transform FindFirstChildNameEndsWith(Transform parent, XRHandJointID jointId)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent), "The parent transform is null.");
            }

            Transform matchChild = null;

            int childCount = parent.childCount;
            string jointIdName = jointId.ToString();

            for (int nextChildIndex = 0; nextChildIndex < childCount; ++nextChildIndex)
            {
                var nextChild = parent.GetChild(nextChildIndex);
                if (nextChild.name.EndsWith(jointIdName))
                {
                    matchChild = nextChild;
                    break;
                }
            }

            if (matchChild == null || !matchChild.name.EndsWith(jointIdName))
            {
                throw new InvalidOperationException($"Hand transform hierarchy not set correctly - couldn't find '{jointId}' joint!");
            }

            return matchChild;
        }

        private static void ToggleRenderers<TRenderer>(bool toggle, Transform xform)
            where TRenderer : Renderer
        {
            if (xform.TryGetComponent<TRenderer>(out var renderer))
            {
                renderer.enabled = toggle;
            }

            for (int childIndex = 0; childIndex < xform.childCount; ++childIndex)
            {
                ToggleRenderers<TRenderer>(toggle, xform.GetChild(childIndex));
            }
        }

        private void AssignJoint(
            XRHandJointID jointId,
            Transform jointXform,
            Transform drawJointsParent,
            GameObject debugDrawPrefab,
            GameObject velocityPrefab)
        {
            int jointIndex = jointId.ToIndex();
            jointXforms[jointIndex] = jointXform;

            drawJoints[jointIndex] = Object.Instantiate(debugDrawPrefab);
            drawJoints[jointIndex].transform.SetParent(drawJointsParent, false);
            drawJoints[jointIndex].name = jointId.ToString();

            velocityParents[jointIndex] = Object.Instantiate(velocityPrefab);
            velocityParents[jointIndex].transform.SetParent(jointXform, false);

            lines[jointIndex] = drawJoints[jointIndex].GetComponent<LineRenderer>();
            lines[jointIndex].startWidth = lines[jointIndex].endWidth = LineWidth;
            linePointsReuse[0] = linePointsReuse[1] = jointXform.position;
            lines[jointIndex].SetPositions(linePointsReuse);
        }

        private void UpdateJoint(
            bool debugDrawJoints,
            VelocityType velocityType,
            Pose originPose,
            XRHandJoint joint,
            ref Pose parentPose,
            bool cacheParentPose = true)
        {
            int jointIndex = joint.id.ToIndex();
            var xform = jointXforms[jointIndex];
            if (xform == null || !joint.TryGetPose(out var pose))
            {
                return;
            }

            drawJoints[jointIndex].transform.SetLocalPositionAndRotation(pose.position, pose.rotation);

            if (debugDrawJoints && joint.id != XRHandJointID.Wrist)
            {
                linePointsReuse[0] = parentPose.GetTransformedBy(originPose).position;
                linePointsReuse[1] = pose.GetTransformedBy(originPose).position;
                lines[jointIndex].SetPositions(linePointsReuse);
            }

            var inverseParentRotation = Quaternion.Inverse(parentPose.rotation);
            var targetLocalPosition = inverseParentRotation * (pose.position - parentPose.position);
            var targetLocalRotation = inverseParentRotation * pose.rotation;
            xform.SetLocalPositionAndRotation(targetLocalPosition, targetLocalRotation);
            if (cacheParentPose)
            {
                parentPose = pose;
            }

            if (velocityType != VelocityType.None && velocityParents[jointIndex].TryGetComponent<LineRenderer>(out var renderer))
            {
                velocityParents[jointIndex].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                linePointsReuse[0] = linePointsReuse[1] = velocityParents[jointIndex].transform.position;
                if (velocityType == VelocityType.Linear)
                {
                    if (joint.TryGetLinearVelocity(out var velocity))
                    {
                        linePointsReuse[1] += velocity;
                    }
                }
                else if (velocityType == VelocityType.Angular)
                {
                    if (joint.TryGetAngularVelocity(out var velocity))
                    {
                        linePointsReuse[1] += 0.05f * velocity.normalized;
                    }
                }

                renderer.SetPositions(linePointsReuse);
            }
        }
    }
}
