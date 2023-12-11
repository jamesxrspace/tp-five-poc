using System;
using MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace TPFive.Home.Entry.SocialLobby
{
    public class PanInteractable : StatefulInteractable
    {
        private Vector3? startInteractionPoint;
        private Vector2 panOffset;
        private IXRInteractor interactingInteractor;

        /// <summary>
        /// panning speed. For example: 0.1f for slow panning. 0.6f for fast panning.
        /// </summary>
        [SerializeField]
        private float panSpeedFactor = 1f;

        public event Action<Vector2> OnPan;

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (interactingInteractor == null)
            {
                return;
            }

            // Dynamic is called when MonoBehaviour.Update().
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                var pos = interactingInteractor.GetAttachTransform(this).position;

                if (!startInteractionPoint.HasValue)
                {
                    startInteractionPoint = pos;
                }
                else
                {
                    // Pan
                    var delta = pos - startInteractionPoint.Value;
                    var projDir = Vector3.ProjectOnPlane(delta, transform.up);
                    projDir = transform.InverseTransformDirection(projDir);
                    panOffset = new Vector2(projDir.x * panSpeedFactor, projDir.z * panSpeedFactor);

                    startInteractionPoint = pos;
                }
            }
            else if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Late)
            {
                OnPan?.Invoke(panOffset);
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            interactingInteractor = args.interactorObject;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            panOffset = default;

            if (args.interactorObject == interactingInteractor)
            {
                interactingInteractor = null;
                startInteractionPoint = null;
            }

            base.OnSelectExited(args);
        }
    }
}