using System;
using System.Collections.Generic;
using TPFive.Game.Avatar.Timeline;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Timeline.TimeMachine
{
    /// <summary>
    /// This class is managed by <see cref="TimeMachineMixerBehaviour"/>.
    /// All TimeMachine behaviour can access this class.
    /// </summary>
    public class TimeMachineController
    {
        public TimeMachineController(IAvatarTimelineManager manager, IEnumerable<TimelineClip> clips)
        {
            Manager = manager;
            var idDict = new Dictionary<int, TimelineClip>();
            var tagDict = new Dictionary<string, TimelineClip>();
            var id = 0;
            foreach (var clip in clips)
            {
                if (clip.asset is not TimeMachinePlayableBase point)
                {
                    continue;
                }

                point.Id = id;
                idDict.TryAdd(point.Id, clip);
                tagDict.TryAdd(point.Tag, clip);
                id++;
            }

            ClipDictionaryById = idDict;
            ClipDictionaryByTag = tagDict;
        }

        public event Action<double> UpdateEvent;

        public IAvatarTimelineManager Manager { get; }

        public IReadOnlyDictionary<int /* Id */, TimelineClip> ClipDictionaryById { get; }

        public IReadOnlyDictionary<string /* Tag */, TimelineClip> ClipDictionaryByTag { get; }

        public void InvokeUpdateEvent()
        {
            UpdateEvent?.Invoke(Manager.Time);
        }

        public void SetTime(float time)
        {
            Manager.Time = time;
        }
    }
}