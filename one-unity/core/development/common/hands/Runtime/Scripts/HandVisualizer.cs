using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;

namespace TPFive.Extended.Hands
{
    /// <summary>
    /// Visual the real hand locomotion.
    /// </summary>
    /// <remarks>
    /// Will appear the hand mesh when hand tracking acquired and disappear when lost.
    /// <br/><br/>
    /// Base on XR Hands package(v1.1.0) - Sample - HandVisualizer - HandVisualizer.cs.
    /// </remarks>
    public class HandVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If this is enabled, this component will enable the Input System internal feature flag 'USE_OPTIMIZED_CONTROLS'. You must have at least version 1.5.0 of the Input System and have its backend enabled for this to take effect.")]
        private bool useOptimizedControls;

        [SerializeField]
        private XROrigin xrOrigin;

        [SerializeField]
        private GameObject leftHandMesh;

        [SerializeField]
        private GameObject rightHandMesh;

        [SerializeField]
        private Material handMeshMaterial;

        [SerializeField]
        private bool drawMeshes;

        [SerializeField]
        private GameObject debugDrawPrefab;

        [SerializeField]
        private bool debugDrawJoints;

        [SerializeField]
        private GameObject velocityPrefab;

        [SerializeField]
        private VelocityType velocityType;

        private bool previousDrawMeshes;
        private bool previousDebugDrawJoints;
        private VelocityType previousVelocityType;

        private XRHandSubsystem handSubsystem;
        private HandGameObjects leftHandGameObjects;
        private HandGameObjects rightHandGameObjects;

        private Coroutine waitHandSubsystemRoutine;

        public bool DrawMeshes
        {
            get => drawMeshes;
            set => drawMeshes = value;
        }

        public bool DebugDrawJoints
        {
            get => debugDrawJoints;
            set => debugDrawJoints = value;
        }

        public VelocityType VelocityType
        {
            get => velocityType;
            set => velocityType = value;
        }

        private static IEnumerator EnsureHandSubsystemLoaded(Action<XRHandSubsystem> onLoadedCallback)
        {
            var handSubsystemCollection = new List<XRHandSubsystem>();
            XRHandSubsystem newHandSubsystem = null;
            do
            {
                SubsystemManager.GetSubsystems(handSubsystemCollection);
                if (handSubsystemCollection.Count != 0)
                {
                    newHandSubsystem = handSubsystemCollection[0];
                    break;
                }

                yield return null;
            }
            while (true);

            onLoadedCallback?.Invoke(newHandSubsystem);
        }

        private void Awake()
        {
            if (useOptimizedControls)
            {
                UnityEngine.InputSystem.InputSystem.settings.SetInternalFeatureFlag("USE_OPTIMIZED_CONTROLS", true);
            }
        }

        private void OnDestroy()
        {
            if (leftHandGameObjects != null)
            {
                leftHandGameObjects.OnDestroy();
                leftHandGameObjects = null;
            }

            if (rightHandGameObjects != null)
            {
                rightHandGameObjects.OnDestroy();
                rightHandGameObjects = null;
            }
        }

        private void OnEnable()
        {
            if (handSubsystem == null)
            {
                waitHandSubsystemRoutine = StartCoroutine(EnsureHandSubsystemLoaded(OnHandSubsystemLoaded));
                return;
            }

            handSubsystem.trackingAcquired += OnTrackingAcquired;
            handSubsystem.trackingLost += OnTrackingLost;
            handSubsystem.updatedHands += OnUpdatedHands;

            UpdateRenderingVisibility(leftHandGameObjects, handSubsystem.leftHand.isTracked);
            UpdateRenderingVisibility(rightHandGameObjects, handSubsystem.rightHand.isTracked);
        }

        private void OnDisable()
        {
            if (waitHandSubsystemRoutine != null)
            {
                StopCoroutine(waitHandSubsystemRoutine);
                waitHandSubsystemRoutine = null;
            }

            if (handSubsystem != null)
            {
                handSubsystem.trackingAcquired -= OnTrackingAcquired;
                handSubsystem.trackingLost -= OnTrackingLost;
                handSubsystem.updatedHands -= OnUpdatedHands;
            }

            UpdateRenderingVisibility(leftHandGameObjects, false);
            UpdateRenderingVisibility(rightHandGameObjects, false);
        }

        private void OnHandSubsystemLoaded(XRHandSubsystem newHandSubsystem)
        {
            handSubsystem = newHandSubsystem;
            handSubsystem.trackingAcquired += OnTrackingAcquired;
            handSubsystem.trackingLost += OnTrackingLost;
            handSubsystem.updatedHands += OnUpdatedHands;

            leftHandGameObjects = new HandGameObjects(
                Handedness.Left,
                transform,
                leftHandMesh,
                handMeshMaterial,
                debugDrawPrefab,
                velocityPrefab);

            rightHandGameObjects = new HandGameObjects(
                Handedness.Right,
                transform,
                rightHandMesh,
                handMeshMaterial,
                debugDrawPrefab,
                velocityPrefab);

            UpdateRenderingVisibility(leftHandGameObjects, handSubsystem.leftHand.isTracked);
            UpdateRenderingVisibility(rightHandGameObjects, handSubsystem.rightHand.isTracked);

            previousDrawMeshes = drawMeshes;
            previousDebugDrawJoints = debugDrawJoints;
            previousVelocityType = velocityType;
        }

        private void OnTrackingAcquired(XRHand hand)
        {
            switch (hand.handedness)
            {
                case Handedness.Left:
                    UpdateRenderingVisibility(leftHandGameObjects, true);
                    break;

                case Handedness.Right:
                    UpdateRenderingVisibility(rightHandGameObjects, true);
                    break;
            }
        }

        private void OnTrackingLost(XRHand hand)
        {
            switch (hand.handedness)
            {
                case Handedness.Left:
                    UpdateRenderingVisibility(leftHandGameObjects, false);
                    break;

                case Handedness.Right:
                    UpdateRenderingVisibility(rightHandGameObjects, false);
                    break;
            }
        }

        private void OnUpdatedHands(
            XRHandSubsystem subsystem,
            XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
            XRHandSubsystem.UpdateType updateType)
        {
            // We have no game logic depending on the Transforms, so early out here
            // (add game logic before this return here, directly querying from
            // subsystem.leftHand and subsystem.rightHand using GetJoint on each hand)
            if (updateType == XRHandSubsystem.UpdateType.Dynamic)
            {
                return;
            }

            bool leftHandTracked = subsystem.leftHand.isTracked;
            bool rightHandTracked = subsystem.rightHand.isTracked;

            if (previousDrawMeshes != drawMeshes)
            {
                leftHandGameObjects.ToggleDrawMesh(drawMeshes && leftHandTracked);
                rightHandGameObjects.ToggleDrawMesh(drawMeshes && rightHandTracked);
                previousDrawMeshes = drawMeshes;
            }

            if (previousDebugDrawJoints != debugDrawJoints)
            {
                leftHandGameObjects.ToggleDebugDrawJoints(debugDrawJoints && leftHandTracked);
                rightHandGameObjects.ToggleDebugDrawJoints(debugDrawJoints && rightHandTracked);
                previousDebugDrawJoints = debugDrawJoints;
            }

            if (previousVelocityType != velocityType)
            {
                leftHandGameObjects.SetVelocityType(leftHandTracked ? velocityType : VelocityType.None);
                rightHandGameObjects.SetVelocityType(rightHandTracked ? velocityType : VelocityType.None);
                previousVelocityType = velocityType;
            }

            leftHandGameObjects.UpdateJoints(
                xrOrigin,
                subsystem.leftHand,
                (updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints) != 0,
                drawMeshes,
                debugDrawJoints,
                velocityType);

            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
            {
                leftHandGameObjects.UpdateRootPose(subsystem.leftHand);
            }

            rightHandGameObjects.UpdateJoints(
                xrOrigin,
                subsystem.rightHand,
                (updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandJoints) != 0,
                drawMeshes,
                debugDrawJoints,
                velocityType);

            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
            {
                rightHandGameObjects.UpdateRootPose(subsystem.rightHand);
            }
        }

        private void UpdateRenderingVisibility(HandGameObjects handGameObjects, bool isTracked)
        {
            if (handGameObjects == null)
            {
                return;
            }

            handGameObjects.ToggleDrawMesh(drawMeshes && isTracked);
            handGameObjects.ToggleDebugDrawJoints(debugDrawJoints && isTracked);
            handGameObjects.SetVelocityType(isTracked ? velocityType : VelocityType.None);
        }
    }
}
