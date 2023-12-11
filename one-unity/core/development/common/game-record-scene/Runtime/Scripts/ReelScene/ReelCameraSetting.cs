using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.Record.Scene
{
    /// <summary>
    /// The setting of camera on reel.
    /// </summary>
    [Serializable]
    public sealed class ReelCameraSetting
    {
        [SerializeField]
        [Tooltip("The GameObject containing \"ICamera\" component.")]
        private GameObject cameraObject;

        [SerializeField]
        [Tooltip("Whether allow interact with move input by user.")]
        private bool allowInteractWithMoveInput;

        [SerializeField]
        [Tooltip("Whether allow interact with rotate input by user.")]
        private bool allowInteractWithRotateInput;

        [SerializeField]
        [Tooltip("How to face the target when camera goto live.")]
        private ReelCameraGotoLiveFacingMode gotoLiveFacingMode;

        [SerializeField]
        [Tooltip("The distance between camera and target.")]
        private float distanceBetweenTarget = 4f;

        public GameObject CameraObject => cameraObject;

        public bool AllowInteractWithMoveInput => allowInteractWithMoveInput;

        public bool AllowInteractWithRotateInput => allowInteractWithRotateInput;

        public ReelCameraGotoLiveFacingMode GotoLiveFacingMode => gotoLiveFacingMode;

        public float DistanceBetweenTarget => distanceBetweenTarget;
    }
}
