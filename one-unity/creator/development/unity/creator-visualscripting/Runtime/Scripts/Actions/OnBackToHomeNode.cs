using System.Collections.Generic;
using MessagePipe;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Creator.VisualScripting
{
    using GameMessages = TPFive.Game.Messages;
    using CreatorMessages = TPFive.Creator.Messages;

    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    [UnitTitle("On Back To Home")]
    [UnitCategory("Events\\XrSpace")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class OnBackToHomeNode : EventUnitBase<EmptyEventArgs>
    {
        private const string EventName = "OnBackToHome";

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventName);
        }

        protected override void Definition()
        {
            base.Definition();
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            return true;
        }
    }
}
