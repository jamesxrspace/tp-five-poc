using MixedReality.Toolkit.Input;
using UnityEngine;

namespace TPFive.Extended.InputXREvent
{
    public interface ITransmitterVR
    {
        public Transform RightHand { get; }

        public Transform LeftHand { get; }

        public PokeInteractor RightPokeInteractor { get; }

        public MRTKRayInteractor RightRayInteractor { get; }

        public GrabInteractor RightGrabInteractor { get; }

        public GazePinchInteractor RightGazePinchInteractor { get; }

        public PokeInteractor LeftPokeInteractor { get; }

        public MRTKRayInteractor LeftRayInteractor { get; }

        public GrabInteractor LeftGrabInteractor { get; }

        public GazePinchInteractor LeftGazePinchInteractor { get; }

        public GazeInteractor GazeInteractor { get; }
    }
}