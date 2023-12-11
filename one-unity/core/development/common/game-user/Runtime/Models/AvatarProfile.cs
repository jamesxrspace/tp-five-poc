using System;

namespace TPFive.Game.User
{
    /// <summary>
    /// Represents an avatar profile containing information about different photos of an avatar.
    /// </summary>
    public class AvatarProfile : IEquatable<AvatarProfile>, IAvatarProfile
    {
        /// <summary>
        /// Gets or sets the unique identifier of the avatar profile.
        /// </summary>
        /// <value>A string of the the unique identifier of the avatar profile.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the xrid of the avatar owner.
        /// </summary>
        /// <value>A string of the owner' xrid.</value>
        public string OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the URL to the binary file of the avatar.
        /// </summary>
        /// <value>The url of the binfile.</value>
        public string BinfileUrl { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the avatar profile was created.
        /// </summary>
        /// <value>The data time of the avatar profile created.</value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the format of the avatar.
        /// </summary>
        /// <value>A value of the avatar format.</value>
        public AvatarFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the URL to the full-body photo of the avatar.
        /// </summary>
        /// <value>The url of the full-body photo.</value>
        public string FullBodyPhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to the half-body photo of the avatar.
        /// </summary>
        /// <value>The url of the half-body photo.</value>
        public string HalfBodyPhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to the headshot photo of the avatar.
        /// </summary>
        /// <value>The url of the headshot photo.</value>
        public string HeadshotPhotoUrl { get; set; }

        /// <summary>
        /// Determines whether the current AvatarProfile object is equal to another AvatarProfile object.
        /// </summary>
        /// <param name="obj">The AvatarProfile object to compare with the current AvatarProfile object.</param>
        /// <returns>The result of equality.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as AvatarProfile);
        }

        /// <summary>
        /// Determines whether the current AvatarProfile object is equal to another AvatarProfile object.
        /// </summary>
        /// <param name="other">The AvatarProfile object to compare with the current AvatarProfile object.</param>
        /// <returns>The result of equality.</returns>
        public bool Equals(AvatarProfile other)
        {
            return other is not null && string.Equals(Id, other.Id, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a hash code for the current AvatarProfile object.
        /// </summary>
        /// <returns>A hash code for the current AvatarProfile object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public bool TryGetFormat<T>(out T format)
            where T : class
        {
            throw new NotImplementedException();
        }
    }
}
