using System;
using UnityEngine;
using UnityEngine.Events;

namespace TPFive.Game.UnityEvents
{
    /// <summary>
    /// Unity event that can carry <see cref="UnityEngine.Vector2"/>.
    /// </summary>
    [Serializable]
    public sealed class Vector2UnityEvent : UnityEvent<Vector2>
    {
    }
}
