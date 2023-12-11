using System;
using System.Collections.Generic;
using TPFive.Game.Camera;
using UnityEngine;

namespace TPFive.Extended.Camera
{
    /// <summary>
    /// Stores the priority of each camera state.
    /// </summary>
    [CreateAssetMenu(fileName = "CinemachinePriorityConfig", menuName = "TPFive/Camera/Create Cinemachine Priority Config")]
    public class CinemachinePriorityConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private StatePriority[] statePriorities;

        private Dictionary<CameraState, CinemachinePriority> statePriorityMap = new Dictionary<CameraState, CinemachinePriority>();

        public CinemachinePriority GetPriority(CameraState state)
        {
            return statePriorityMap.TryGetValue(state, out var priority) ? priority : CinemachinePriority.Zero;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            statePriorityMap.Clear();

            if (statePriorities == null)
            {
                return;
            }

            foreach (var statePriority in statePriorities)
            {
                statePriorityMap.Add(statePriority.State, statePriority.Priority);
            }
        }

        [Serializable]
        private struct StatePriority
        {
            [SerializeField]
            private CameraState state;

            [SerializeField]
            private CinemachinePriority priority;

            public CameraState State => state;

            public CinemachinePriority Priority => priority;
        }
    }
}
