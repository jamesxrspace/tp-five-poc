using TPFive.Game.Avatar.Attachment;
using UnityEngine;

namespace TPFive.Game.Avatar.Factory
{
    /// <summary>
    /// This creator is used to create an IAnchorPointProvider.
    /// </summary>
    public static class AvatarAnchorPointCreator
    {
        public static IAnchorPointProvider Create(
            GameObject root,
            AnchorPointDefinition[] definitions,
            Animator animator)
        {
            return new AvatarAnchorPointProvider(root.transform, animator, definitions);
        }
    }
}