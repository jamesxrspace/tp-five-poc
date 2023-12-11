using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public interface ILayout
    {
        /// <summary>
        /// Size of the model for calculating the layout.
        /// This size should encapsulate both entity's collider size and model size.
        /// </summary>
        Vector3 Size { get; }

        /// <summary>
        /// Sort order before the layout
        /// FIXME:
        /// Remove this property after API pagination is implemented.
        /// See discussion: https://github.com/XRSPACE-Inc/tp-five/pull/335#discussion_r1326679190
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// The local position of the entity in entity manager.
        /// </summary>
        Vector3 LocalPosition { get; set; }
    }
}