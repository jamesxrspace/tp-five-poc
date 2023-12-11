using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace TPFive.Extended.Hands
{
    /// <summary>
    /// Manage the hand devices interactors.
    /// </summary>
    /// <remarks>
    /// Will toggle on when using hand tracking and off when lost.
    /// <br/><br/>
    /// Base on XR Interaction Toolkit package(v2.3.2) - Sample - Hands Interaction Demo - HandsAndControllersManager.cs.
    /// </remarks>
    public class HandsDeviceManager : MonoBehaviour
    {
        [Header("Hand Groups")]
        [SerializeField]
        [Tooltip("GameObject representing the left hand group of interactors. Will toggle on when using hand tracking and off when lost.")]
        private GameObject leftHandGroup;

        [SerializeField]
        [Tooltip("GameObject representing the right hand group of interactors. Will toggle on when using hand tracking and off when lost.")]
        private GameObject rightHandGroup;

        private IDictionary<Handedness, GameObject> handGroupDict = new Dictionary<Handedness, GameObject>();

        private XRHandSubsystem handSubsystem;
        private Coroutine waitHandSubsystemRoutine;

        private static IEnumerator EnsureHandSubsystemLoaded(Action<XRHandSubsystem> onLoadedCallback)
        {
            var handSubsystemCollection = new List<XRHandSubsystem>();
            XRHandSubsystem newHandSubsystem = null;
            yield return new WaitUntil(() =>
            {
                SubsystemManager.GetSubsystems(handSubsystemCollection);
                if (handSubsystemCollection.Count == 0)
                {
                    return false;
                }

                newHandSubsystem = handSubsystemCollection[0];
                return true;
            });

            onLoadedCallback?.Invoke(newHandSubsystem);
        }

        private void Awake()
        {
            handGroupDict.Add(Handedness.Left, leftHandGroup);
            handGroupDict.Add(Handedness.Right, rightHandGroup);

            ToggleHandGroup(Handedness.Left, false);
            ToggleHandGroup(Handedness.Right, false);
        }

        private void OnDestroy()
        {
            handGroupDict.Clear();
        }

        private void OnEnable()
        {
            if (handSubsystem != null)
            {
                if (handSubsystem.leftHand.isTracked)
                {
                    OnHandTrackingAcquired(handSubsystem.leftHand);
                }

                if (handSubsystem.rightHand.isTracked)
                {
                    OnHandTrackingAcquired(handSubsystem.rightHand);
                }
            }

            SubscribeHandSubSystemEvents();
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
                OnHandTrackingLost(handSubsystem.leftHand);
                OnHandTrackingLost(handSubsystem.rightHand);
            }

            UnsubscribeHandSubSystemEvents();
        }

        private void Start()
        {
            waitHandSubsystemRoutine = StartCoroutine(EnsureHandSubsystemLoaded(OnHandSubsystemLoaded));
        }

        private void SubscribeHandSubSystemEvents()
        {
            if (handSubsystem == null)
            {
                return;
            }

            handSubsystem.trackingAcquired += OnHandTrackingAcquired;
            handSubsystem.trackingLost += OnHandTrackingLost;
        }

        private void UnsubscribeHandSubSystemEvents()
        {
            if (handSubsystem == null)
            {
                return;
            }

            handSubsystem.trackingAcquired -= OnHandTrackingAcquired;
            handSubsystem.trackingLost -= OnHandTrackingLost;
        }

        private void OnHandTrackingAcquired(XRHand hand)
        {
            ToggleHandGroup(hand.handedness, true);
        }

        private void OnHandTrackingLost(XRHand hand)
        {
            ToggleHandGroup(hand.handedness, false);
        }

        private void ToggleHandGroup(Handedness handedness, bool isOn)
        {
            if (!handGroupDict.TryGetValue(handedness, out GameObject handednessGroup))
            {
                Debug.LogWarning($"[{nameof(HandsDeviceManager)}] Not supports toggle the hand group when handedness is '{handedness}'.", this);
                return;
            }

            if (handednessGroup == null)
            {
                Debug.LogWarning($"[{nameof(HandsDeviceManager)}] The hand group is null. handedness: {handedness}", this);
                return;
            }

            if (handednessGroup.activeSelf == isOn)
            {
                return;
            }

            handednessGroup.SetActive(isOn);
        }

        private void OnHandSubsystemLoaded(XRHandSubsystem newHandSubSystem)
        {
            handSubsystem = newHandSubSystem;

            if (this.enabled)
            {
                OnEnable();
            }
        }
    }
}
