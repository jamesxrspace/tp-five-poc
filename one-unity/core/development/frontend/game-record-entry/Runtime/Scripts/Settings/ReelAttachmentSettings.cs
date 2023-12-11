using System.Linq;
using UnityEngine;

namespace TPFive.Game.Record.Entry.Settings
{
    [CreateAssetMenu(fileName = nameof(ReelAttachmentSettings), menuName = "TPFive/Record/" + nameof(ReelAttachmentSettings))]
    public class ReelAttachmentSettings : ScriptableObject
    {
        [SerializeField]
        private AttachmentPair[] attachmentPairs;

        public AttachmentPair[] AttachmentPairs => attachmentPairs;

        public bool TryGetCategory(ReelAvatarMotionMode mode, out string category)
        {
            category = null;

            // Lookup the attachment category by AvatarMotionMode.
            // This category is setup in Reel Entry Package.
            category = attachmentPairs.FirstOrDefault(x => x.AvatarMotionMode == mode)?.Category;

            return category != null;
        }
    }
}