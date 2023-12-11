using UnityEngine.UI;

namespace TPFive.UI
{
    /// <summary>
    /// Provide a component for blocking raycasts without any rendering.
    /// </summary>
    /// <remarks>
    /// A concrete subclass of the Unity UI `Graphic` class that just skips drawing.
    /// Useful for providing an raycast target without actually drawing anything.
    /// </remarks>
    public sealed class UIRaycastBlockable : Graphic
    {
        // Overrides SetMaterialDirty() with no custom action.
        public override void SetMaterialDirty()
        {
            // Nothing to do.
        }

        // Overrides SetVerticesDirty() with no custom action.
        public override void SetVerticesDirty()
        {
            // Nothing to do.
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    } // END class
} // END namespace
