using System;
using MixedReality.Toolkit;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace TPFive.Home.Entry.SocialLobby
{
    public class EntityManipulatorMobile : StatefulInteractable
    {
        [SerializeField]
        private Rigidbody rigidBody;

        [SerializeField]
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform = null;

        [SerializeField]
        [Tooltip("Constraint manager slot to enable constraints when manipulating the object.")]
        private ConstraintManager constraintsManager;

        [SerializeField]
        [Tooltip("The concrete type of ManipulationLogic<Vector3> to use for moving.")]
        [Extends(typeof(ManipulationLogic<Vector3>), TypeGrouping.ByNamespaceFlat)]
        private SystemType moveLogicType;

        [SerializeField]
        [Tooltip(
            "Enable or disable constraint support of this component. When enabled," +
            "transform changes will be post processed by the linked constraint manager.")]
        private bool enableConstraints = true;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip(
            "How the scene stick with the touch finger, " +
            "0: the scene barely follow the finger at all, " +
            "1: the scene catch up the finger position as soon as possible")]
        private float normalizedSnappiness = 0.5f;

        [SerializeField]
        private float clickThreshold = 0.5f;

        [SerializeField]
        private float basePosDelta = 0.5f;

        private float selectTimestamp;
        private Vector3 selectEnterPosition;
        private IXRSelectInteractor interactor;
        private MixedRealityTransform targetTransform;
        private ManipulationLogic<Vector3> moveLogic;

        public float ClickThreshold
        {
            get => clickThreshold;
            set => clickThreshold = value;
        }

        public Rigidbody Rigidbody
        {
            get => rigidBody;
            set => rigidBody = value;
        }

        public UnityEvent OnEntityClicked { get; } = new UnityEvent();

        public Transform HostTransform
        {
            get => hostTransform == null ? transform : hostTransform;

            set
            {
                if (interactorsSelecting.Count != 0)
                {
                    return;
                }

                if (hostTransform != value)
                {
                    hostTransform = value;

                    if (constraintsManager != null)
                    {
                        constraintsManager.Setup(new MixedRealityTransform(HostTransform));
                    }

                    rigidBody = HostTransform.GetComponent<Rigidbody>();
                }
            }
        }

        public bool EnableConstraints
        {
            get => enableConstraints;
            set => enableConstraints = value;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (!isSelected)
            {
                return;
            }

            if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                return;
            }

            bool isOneHanded = interactorsSelecting.Count == 1;

            targetTransform = new MixedRealityTransform(
                HostTransform.position,
                HostTransform.rotation,
                HostTransform.localScale);

            targetTransform.Position = moveLogic.Update(
                interactorsSelecting,
                this,
                targetTransform,
                centeredAnchor: false);

            // Immediately apply translation constraints after computing the user's desired scale input.
            if (EnableConstraints && constraintsManager != null)
            {
                constraintsManager.ApplyTranslationConstraints(ref targetTransform, isOneHanded, IsGrabSelected);
            }

            ApplyTargetTransform();
        }

        protected override void Awake()
        {
            base.Awake();

            moveLogic = Activator.CreateInstance(moveLogicType) as ManipulationLogic<Vector3>;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            interactor = args.interactorObject;

            selectTimestamp = Time.time;

            targetTransform = new MixedRealityTransform(
                HostTransform.position,
                HostTransform.rotation,
                HostTransform.localScale);

            moveLogic.Setup(interactorsSelecting, this, targetTransform);

            if (constraintsManager != null && EnableConstraints)
            {
                constraintsManager.OnManipulationStarted(targetTransform);
            }

            if (rigidBody != null)
            {
                rigidBody.velocity = Vector3.zero;
            }

            selectEnterPosition = HostTransform.position;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            var isSameInteractor = interactor == args.interactorObject;
            var isSamePosition = selectEnterPosition == HostTransform.position;

            if (isSameInteractor && isSamePosition && Time.time - selectTimestamp < clickThreshold)
            {
                OnEntityClicked?.Invoke();
            }

            interactor = null;

            // Throw out the rigidbody once all the fingers leave
            if (rigidBody != null && interactorsSelecting.Count == 0)
            {
                var posDelta = targetTransform.Position - HostTransform.position;
                var velocity = posDelta.magnitude / basePosDelta * rigidBody.maxLinearVelocity;
                var direction = posDelta.normalized;

                rigidBody.velocity = direction * velocity;
            }
        }

        private void ApplyTargetTransform()
        {
            var fromPosition = HostTransform.position;
            var toPosition = targetTransform.Position;

            // maybe we should use frame-base lerp instead of Time.deltaTime,
            // but it seems that frame-base lerp will cause the object to shake in unity editor during dragging.
            var lerpPosition = Vector3.Lerp(fromPosition, toPosition, ConvertToSnappiness(normalizedSnappiness) * Time.deltaTime);
            HostTransform.SetPositionAndRotation(lerpPosition, targetTransform.Rotation);

            HostTransform.localScale = targetTransform.Scale;
        }

        private float ConvertToSnappiness(float normalizedValue)
        {
            // experimentally, we found that the snappiness value should be in range [10, 30]
            // 0 -> 10
            // 1 -> 30
            return (normalizedValue * 20) + 10;
        }
    }
}