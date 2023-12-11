using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.Record.Scene
{
    /// <summary>
    /// The setting of state on reel.
    /// </summary>
    [Serializable]
    public sealed class ReelStateSetting
    {
        [Header("General")]
        [SerializeField]
        [Tooltip("My state")]
        private ReelState state;

        [SerializeField]
        [Tooltip("Align which state")]
        private Optional<ReelState> alignState;

        [Header("Camera - Default Setting")]
        [SerializeField]
        private bool enableSingleDefaultCamera;

        [SerializeField]
        private ReelCameraSetting singleDefaultCameraSetting;

        [SerializeField]
        private List<ReelCameraSetting> multiDefaultCameraSettings;

        public ReelStateSetting(ReelState state)
        {
            this.state = state;
        }

        public ReelStateSetting()
        {
        }

        public ReelState State => state;

        public Optional<ReelState> AlignState => alignState;

        public bool EnableSingleDefaultCamera => enableSingleDefaultCamera;

        public ReelCameraSetting SingleDefaultCameraSetting => singleDefaultCameraSetting;

        public List<ReelCameraSetting> MultiDefaultCameraSettings => multiDefaultCameraSettings;
    }
}
