using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace TPFive.Creator
{
    public abstract class MarkerBase :
        UnityEngine.Timeline.Marker,
        INotification
    {
        public PropertyName id { get; }
    }
}
