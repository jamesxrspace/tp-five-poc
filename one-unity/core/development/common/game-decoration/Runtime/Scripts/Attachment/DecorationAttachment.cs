using System;
using TPFive.Game.Decoration.Attachment;
using UnityEngine;

namespace TPFive.Game.Decoration
{
    public class DecorationAttachment : MonoBehaviour
    {
        [SerializeField]
        private AnchorPointDefinition[] anchorPointDefinition;

        public AnchorPointDefinition[] AnchorPointDefinition => anchorPointDefinition;

        public bool TryGetDefinition(string category, out AnchorPointDefinition definition)
        {
            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentException($"{nameof(category)} cannot be null or empty.", nameof(category));
            }

            definition = null;

            if (AnchorPointDefinition == null)
            {
                return false;
            }

            // Lookup the anchor point definition by category (ex. Predefine, Custom tag).
            // This definition is setup from content project in each decoration attachment.
            definition = Array.Find(AnchorPointDefinition, x => category.Equals(x.Category, StringComparison.OrdinalIgnoreCase));

            return definition != null;
        }
    }
}