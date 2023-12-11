using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace TPFive.Creator.Components.Helper
{
    /// <summary>
    /// This is a test class for get MRTK's FarRay Interactor and apply to SolverHandler
    /// It will be replaced by our InputFramework that can get easy to get the target interactor in the future.
    /// </summary>
    [RequireComponent(typeof(StatefulInteractable), typeof(SolverHandler), typeof(TapToPlace))]
    public class SolverHandlerHelper : MonoBehaviour
    {
        [SerializeField]
        private SolverHandler solverHandler;
        [SerializeField]
        private StatefulInteractable statefulInteractable;
        [SerializeField]
        private TapToPlace tapToPlace;
        [SerializeField]
        private LayerMask[] magneticSurfaces;

        protected void Start()
        {
            Init();
        }

        protected void OnDestroy()
        {
            if (statefulInteractable != null)
            {
                statefulInteractable.hoverEntered.RemoveListener(ApplyInteractorToSolver);
            }
        }

        private void Init()
        {
            statefulInteractable.hoverEntered.RemoveListener(ApplyInteractorToSolver);
            statefulInteractable.hoverEntered.AddListener(ApplyInteractorToSolver);
            tapToPlace.MagneticSurfaces = magneticSurfaces;
            solverHandler.TrackedTargetType = TrackedObjectType.Interactor;
        }

        // TODO : The input action should be replace by TPFive InputFramework in the future.
        private void ApplyInteractorToSolver(HoverEnterEventArgs args)
        {
            if (args.interactorObject is not XRRayInteractor rayInteractor)
            {
                return;
            }

            if (rayInteractor.xrController is not ArticulatedHandController controller)
            {
                return;
            }

            if (controller.HandNode == XRNode.RightHand)
            {
                solverHandler.RightInteractor = rayInteractor;
            }
            else if (controller.HandNode == XRNode.LeftHand)
            {
                solverHandler.LeftInteractor = rayInteractor;
            }
        }
    }
}