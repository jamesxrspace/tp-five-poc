using System;
using UnityEngine;

namespace TPFive.Game.Avatar
{
    public class AnimationIKExecutor : MonoBehaviour
    {
        public event Action<int> OnAnimatorIKUpdate;

        /// <summary>
        /// It is called by the Animator Component immediately before it updates its internal IK system.
        /// </summary>
        protected void OnAnimatorIK(int layerIndex)
        {
            OnAnimatorIKUpdate?.Invoke(layerIndex);
        }
    }
}