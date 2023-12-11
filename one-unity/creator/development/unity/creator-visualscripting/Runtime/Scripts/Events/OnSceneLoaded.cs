using System.Collections.Generic;
using MessagePipe;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Creator.VisualScripting
{
    //
    using CreatorMessages = TPFive.Creator.Messages;

    //
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    [UnitTitle("On Scene Loaded")]
    [UnitCategory("Events\\XrSpace")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class OnSceneLoadedNode : EventUnitBase<Scene>
    {
        private const string EventName = "OnSceneLoaded";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput scene { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventName);
        }

        protected override void Definition()
        {
            base.Definition();
            scene = ValueOutput<Scene>(nameof(scene));
        }

        protected override bool ShouldTrigger(Flow flow, Scene args)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, Scene args)
        {
            flow.SetValue(scene, args);
        }
    }
}
