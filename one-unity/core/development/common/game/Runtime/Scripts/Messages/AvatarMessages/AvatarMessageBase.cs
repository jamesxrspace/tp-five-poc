using UnityEngine;

namespace TPFive.Game.Messages
{
    /// <summary>
    /// base class of avatar message.
    /// </summary>
    public abstract class AvatarMessageBase
    {
        protected AvatarMessageBase(GameObject root)
        {
            Root = root;
        }

        public GameObject Root { get; }
    }
}
