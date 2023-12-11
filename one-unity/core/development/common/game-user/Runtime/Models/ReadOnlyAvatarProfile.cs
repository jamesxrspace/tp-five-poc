using System;
using Newtonsoft.Json.Linq;
using TPFive.OpenApi.GameServer.Model;

namespace TPFive.Game.User
{
    public class ReadOnlyAvatarProfile : IAvatarProfile
    {
        private readonly AvatarMetadata metadata;
        private AvatarFormat avatarFormat;

        internal ReadOnlyAvatarProfile(AvatarMetadata metadata)
        {
            this.metadata = metadata;
        }

        /// <summary>
        /// Gets the unique identifier of the avatar profile.
        /// </summary>
        /// <value>A string of the the unique identifier of the avatar profile.</value>
        public string Id => metadata.AvatarId;

        /// <summary>
        /// Gets the xrid of the avatar owner.
        /// </summary>
        /// <value>A string of the owner' xrid.</value>
        public string OwnerId => metadata.Xrid;

        /// <summary>
        /// Gets the URL to the binary file of the avatar.
        /// </summary>
        /// <value>The url of the binfile.</value>
        public string BinfileUrl => metadata.AvatarUrl;

        /// <summary>
        /// Gets the date and time when the avatar profile was created.
        /// </summary>
        /// <value>The data time of the avatar profile created.</value>
        public DateTime CreatedAt => metadata.CreatedAt;

        /// <summary>
        /// Gets the format of the avatar.
        /// </summary>
        /// <value>A value of the avatar format.</value>
        public AvatarFormat Format
        {
            get
            {
                if (avatarFormat == null
                    && metadata.AvatarFormat is JObject obj)
                {
                    avatarFormat = obj.ToObject<AvatarFormat>();
                }

                return avatarFormat;
            }
        }

        /// <summary>
        /// Gets the URL to the full-body photo of the avatar.
        /// </summary>
        /// <value>The url of the full-body photo.</value>
        public string FullBodyPhotoUrl => metadata.Thumbnail?.FullBody;

        /// <summary>
        /// Gets the URL to the half-body photo of the avatar.
        /// </summary>
        /// <value>The url of the half-body photo.</value>
        public string HalfBodyPhotoUrl => metadata.Thumbnail?.UpperBody;

        /// <summary>
        /// Gets the URL to the headshot photo of the avatar.
        /// </summary>
        /// <value>The url of the headshot photo.</value>
        public string HeadshotPhotoUrl => metadata.Thumbnail?.Head;
    }
}