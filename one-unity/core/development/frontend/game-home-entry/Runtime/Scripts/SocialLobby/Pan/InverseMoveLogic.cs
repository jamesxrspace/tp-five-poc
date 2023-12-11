using System.Collections.Generic;
using MixedReality.Toolkit;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace TPFive.Home.Entry.SocialLobby
{
    public class InverseMoveLogic : ManipulationLogic<Vector3>
    {
        private Vector3 attachToObject;
        private Vector3 startAttachCentroid;

        /// <inheritdoc />
        public override void Setup(
            List<IXRSelectInteractor> interactors,
            IXRSelectInteractable interactable,
            MixedRealityTransform currentTarget)
        {
            base.Setup(interactors, interactable, currentTarget);

            startAttachCentroid = GetAttachCentroid(interactors, interactable);

            attachToObject = currentTarget.Position - startAttachCentroid;
        }

        /// <inheritdoc />
        public override Vector3 Update(
            List<IXRSelectInteractor> interactors,
            IXRSelectInteractable interactable,
            MixedRealityTransform currentTarget,
            bool centeredAnchor)
        {
            base.Update(interactors, interactable, currentTarget, centeredAnchor);

            Vector3 attachCentroid = GetAttachCentroid(interactors, interactable);

            if (centeredAnchor)
            {
                return attachCentroid + attachToObject;
            }
            else
            {
                return currentTarget.Position - (attachCentroid - startAttachCentroid);
            }
        }

        private Vector3 GetAttachCentroid(List<IXRSelectInteractor> interactors, IXRSelectInteractable interactable)
        {
            // TODO: This uses the attachTransform ONLY, which can possibly be
            // unstable/imprecise (see GrabInteractor, etc.) Old version used to use the interactor
            // transform in the case where there was only one interactor, and the attachTransform
            // when there were 2+. The interactor should stabilize its attachTransform
            // to get a similar effect. Possibly, we should stabilize grabs on the thumb, or some
            // other technique.
            Vector3 sumPos = Vector3.zero;
            int count = 0;
            foreach (IXRSelectInteractor interactor in interactors)
            {
                sumPos += interactor.GetAttachTransform(interactable).position;
                count++;
            }

            return sumPos / Mathf.Max(1, count);
        }
    }
}