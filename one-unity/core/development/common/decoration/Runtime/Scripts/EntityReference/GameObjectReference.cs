using UnityEngine;

namespace TPFive.Extended.Decoration
{
    /// <summary>
    /// Calculate how many entities this gameObject has
    /// and store its related information (BundleId).
    /// </summary>
    public class GameObjectReference : EntityReference<GameObject>
    {
        public GameObjectReference(string bundleId)
            : base(id: bundleId)
        {
        }

        // This bundle id is used to unload asset.
        public string BundleId => ID;
    }
}