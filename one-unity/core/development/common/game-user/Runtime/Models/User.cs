using System;
using System.Collections.Generic;

namespace TPFive.Game.User
{
    public sealed class User : IEquatable<User>
    {
        private string uid;
        private string id;
        private string nickname;

        /// <summary>
        /// Gets an unique id of the user.
        /// </summary>
        /// <value>The unique id of the user.</value>
        public string Uid
        {
            get => uid;
            internal set => uid = value;
        }

        /// <summary>
        /// Gets a id of the user.
        /// </summary>
        /// <value>The id of the user.</value>
        public string Id
        {
            get => id;
            internal set => id = value;
        }

        /// <summary>
        /// Gets or Sets the nickname of the user.
        /// </summary>
        /// <value>The nickname of the user.</value>
        public string Nickame
        {
            get => nickname;
            set => nickname = value;
        }

        public static bool operator ==(User left, User right)
        {
            return EqualityComparer<User>.Default.Equals(left, right);
        }

        public static bool operator !=(User left, User right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public bool Equals(User other)
        {
            return other is not null &&
                   uid == other.uid;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(uid);
        }
    }
}
