using TPFive.Game.Interactable.Toolkit;
using UnityEngine;

namespace TPFive.Creator.Components.Interactable
{
    /// <inheritdoc cref="TPFive.Game.Interactable.Toolkit.IControllable" />
    public class Controllable : MonoBehaviour, IControllable
    {
        /// <inheritdoc/>
        public bool IsControlled { get; private set; }

        /// <inheritdoc/>
        [ContextMenu(nameof(Control))]
        public virtual void Control()
        {
            IsControlled = true;
        }

        /// <inheritdoc/>
        [ContextMenu(nameof(Release))]
        public virtual void Release()
        {
            IsControlled = false;
        }
    }
}