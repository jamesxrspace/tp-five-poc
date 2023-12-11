using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace TPFive.Extended.InputDeviceProvider
{
    /// <summary>
    /// Use this class to mediate the controllers and their associated interactors and input actions under
    /// different interaction states.
    /// <br/><br/>
    /// Base on XR Interaction Toolkit package(v2.3.2) - Sample - Starter Assets - ActionBasedControllerManager.cs.
    /// </summary>
    public class ActionBasedControllerManager : MonoBehaviour
    {
        /// <summary>
        /// Temporary scratch list to populate with the group members of the interaction group.
        /// </summary>
        private static readonly List<IXRGroupMember> GroupMembers = new List<IXRGroupMember>();

        [Header("Interactors")]
        [SerializeField]
        [Tooltip("The GameObject containing the interaction group used for direct and distant manipulation.")]
        private XRInteractionGroup manipulationInteractionGroup;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for direct manipulation.")]
        private XRDirectInteractor directInteractor;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for distant/ray manipulation.")]
        private XRRayInteractor rayInteractor;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for teleportation.")]
        private XRRayInteractor teleportInteractor;

        [Space]
        [Header("Controller Actions")]
        [SerializeField]
        [Tooltip("The reference to the action to start the teleport aiming mode for this controller.")]
        private InputActionReference teleportModeActivate;

        [SerializeField]
        [Tooltip("The reference to the action to cancel the teleport aiming mode for this controller.")]
        private InputActionReference teleportModeCancel;

        [SerializeField]
        [Tooltip("The reference to the action of continuous turning the XR Origin with this controller.")]
        private InputActionReference turn;

        [SerializeField]
        [Tooltip("The reference to the action of snap turning the XR Origin with this controller.")]
        private InputActionReference snapTurn;

        [SerializeField]
        [Tooltip("The reference to the action of moving the XR Origin with this controller.")]
        private InputActionReference move;

        [Space]
        [Header("Locomotion Settings")]
        [SerializeField]
        [Tooltip("If true, continuous movement will be enabled. If false, teleport will enabled.")]
        private bool smoothMotionEnabled;

        [SerializeField]
        [Tooltip("If true, continuous turn will be enabled. If false, snap turn will be enabled. Note: If smooth motion is enabled and enable strafe is enabled on the continuous move provider, turn will be overriden in favor of strafe.")]
        private bool smoothTurnEnabled;

        private bool teleporting;

        public bool SmoothMotionEnabled
        {
            get => smoothMotionEnabled;
            set
            {
                smoothMotionEnabled = value;
                UpdateLocomotionActions();
            }
        }

        public bool SmoothTurnEnabled
        {
            get => smoothTurnEnabled;
            set
            {
                smoothTurnEnabled = value;
                UpdateLocomotionActions();
            }
        }

        protected void Awake()
        {
            // Start the coroutine that executes code after the Update phase (during yield null).
            // This routine is started during Awake to ensure the code after
            // the first yield will execute after Update but still on the first frame.
            // If started in Start, Unity would not resume execution until the second frame.
            // See https://docs.unity3d.com/Manual/ExecutionOrder.html
            StartCoroutine(OnAfterInteractionEvents());
        }

        protected void OnEnable()
        {
            if (teleportInteractor != null)
            {
                teleportInteractor.gameObject.SetActive(false);
            }

            SetupInteractorEvents();
        }

        protected void OnDisable()
        {
            TeardownInteractorEvents();
        }

        protected void Start()
        {
            // Ensure the enabled state of locomotion and turn actions are properly set up.
            // Called in Start so it is done after the InputActionManager enables all input actions earlier in OnEnable.
            UpdateLocomotionActions();

            if (manipulationInteractionGroup == null)
            {
                Debug.LogError("Missing required Manipulation Interaction Group reference. Use the Inspector window to assign the XR Interaction Group component reference.", this);
                return;
            }

            // Ensure interactors are properly set up in the interaction group by adding
            // them if necessary and ordering Direct before Ray interactor.
            var directInteractorIndex = -1;
            var rayInteractorIndex = -1;
            manipulationInteractionGroup.GetGroupMembers(GroupMembers);
            for (var i = 0; i < GroupMembers.Count; ++i)
            {
                var groupMember = GroupMembers[i];
                if (ReferenceEquals(groupMember, directInteractor))
                {
                    directInteractorIndex = i;
                }
                else if (ReferenceEquals(groupMember, rayInteractor))
                {
                    rayInteractorIndex = i;
                }
            }

            /* Make sure two things:
             * 1. Make sure both interactors are in the group.
             * 2. Make sure Direct interactor is ordered before the Ray interactor.
             */
            if (rayInteractorIndex < 0)
            {
                if (directInteractorIndex < 0)
                {
                    manipulationInteractionGroup.AddGroupMember(directInteractor);
                }

                manipulationInteractionGroup.AddGroupMember(rayInteractor);
            }
            else if (directInteractorIndex < 0)
            {
                manipulationInteractionGroup.AddGroupMember(directInteractor);
                manipulationInteractionGroup.MoveGroupMemberTo(directInteractor, rayInteractorIndex);
            }
            else if (rayInteractorIndex < directInteractorIndex)
            {
                manipulationInteractionGroup.MoveGroupMemberTo(directInteractor, rayInteractorIndex);
            }
        }

        private static void SetEnabled(InputActionReference actionReference, bool enabled)
        {
            if (enabled)
            {
                EnableAction(actionReference);
            }
            else
            {
                DisableAction(actionReference);
            }
        }

        private static void EnableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            if (action != null && !action.enabled)
            {
                action.Enable();
            }
        }

        private static void DisableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            if (action != null && action.enabled)
            {
                action.Disable();
            }
        }

        private static InputAction GetInputAction(InputActionReference actionReference)
        {
            return actionReference != null ? actionReference.action : null;
        }

        private IEnumerator OnAfterInteractionEvents()
        {
            // Avoid comparison to null each frame since that operation is somewhat expensive
            if (teleportInteractor == null)
            {
                yield break;
            }

            while (true)
            {
                // Yield so this coroutine is resumed after the teleport interactor
                // has a chance to process its select interaction event.
                yield return null;

                if (!teleporting && teleportInteractor.gameObject.activeSelf)
                {
                    teleportInteractor.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateLocomotionActions()
        {
            // Disable/enable Teleport and Turn when Move is enabled/disabled.
            SetEnabled(move, smoothMotionEnabled);
            SetEnabled(teleportModeActivate, !smoothMotionEnabled);
            SetEnabled(teleportModeCancel, !smoothMotionEnabled);

            // Disable ability to turn when using continuous movement
            SetEnabled(turn, !smoothMotionEnabled && smoothTurnEnabled);
            SetEnabled(snapTurn, !smoothMotionEnabled && !smoothTurnEnabled);
        }

        private void DisableLocomotionActions()
        {
            DisableAction(move);
            DisableAction(teleportModeActivate);
            DisableAction(teleportModeCancel);
            DisableAction(turn);
            DisableAction(snapTurn);
        }

        // For our input mediation, we are enforcing a few rules between direct, ray, and teleportation interaction:
        // 1. If the Teleportation Ray is engaged, the Ray interactor is disabled
        // 2. The interaction group ensures that the Direct and Ray interactors cannot interact at the same time, with the Direct interactor taking priority
        // 3. If the Ray interactor is selecting, all locomotion controls are disabled (teleport ray, move, and turn controls) to prevent input collision
        private void SetupInteractorEvents()
        {
            if (rayInteractor != null)
            {
                rayInteractor.selectEntered.AddListener(OnRaySelectEntered);
                rayInteractor.selectExited.AddListener(OnRaySelectExited);
            }

            var teleportModeActivateAction = GetInputAction(teleportModeActivate);
            if (teleportModeActivateAction != null)
            {
                teleportModeActivateAction.performed += OnStartTeleport;
                teleportModeActivateAction.canceled += OnCancelTeleport;
            }

            var teleportModeCancelAction = GetInputAction(teleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed += OnCancelTeleport;
            }
        }

        private void TeardownInteractorEvents()
        {
            if (rayInteractor != null)
            {
                rayInteractor.selectEntered.RemoveListener(OnRaySelectEntered);
                rayInteractor.selectExited.RemoveListener(OnRaySelectExited);
            }

            var teleportModeActivateAction = GetInputAction(teleportModeActivate);
            if (teleportModeActivateAction != null)
            {
                teleportModeActivateAction.performed -= OnStartTeleport;
                teleportModeActivateAction.canceled -= OnCancelTeleport;
            }

            var teleportModeCancelAction = GetInputAction(teleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed -= OnCancelTeleport;
            }
        }

        private void OnStartTeleport(InputAction.CallbackContext context)
        {
            teleporting = true;

            if (teleportInteractor != null)
            {
                teleportInteractor.gameObject.SetActive(true);
            }

            RayInteractorUpdate();
        }

        private void OnCancelTeleport(InputAction.CallbackContext context)
        {
            teleporting = false;

            // Do not deactivate the teleport interactor in this callback.
            // We delay turning off the teleport interactor in this callback so that
            // the teleport interactor has a chance to complete the teleport if needed.
            // OnAfterInteractionEvents will handle deactivating its GameObject.
            RayInteractorUpdate();
        }

        private void RayInteractorUpdate()
        {
            if (rayInteractor != null)
            {
                rayInteractor.gameObject.SetActive(!teleporting);
            }
        }

        private void OnRaySelectEntered(SelectEnterEventArgs args)
        {
            // Disable locomotion and turn actions
            DisableLocomotionActions();
        }

        private void OnRaySelectExited(SelectExitEventArgs args)
        {
            // Re-enable the locomotion and turn actions
            UpdateLocomotionActions();
        }
    }
}