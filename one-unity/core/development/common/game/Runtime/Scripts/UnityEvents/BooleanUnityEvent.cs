using System;
using UnityEngine.Events;

namespace TPFive.Game.UnityEvents
{
    /// <summary>
    /// Unity event that can carry <see cref="bool"/>.
    /// </summary>
    [Serializable]
    public sealed class BooleanUnityEvent : UnityEvent<bool>
    {
    }
}
