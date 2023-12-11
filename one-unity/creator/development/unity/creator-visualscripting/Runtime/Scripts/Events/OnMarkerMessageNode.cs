using System.Collections.Generic;
using MessagePipe;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace TPFive.Creator.VisualScripting
{
    //
    using CreatorMessages = TPFive.Creator.Messages;

    //
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    [UnitTitle("On Marker Message")]
    [UnitCategory("Events\\XrSpace")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class OnMarkerMessageNode : EventUnitBase<(List<int>, List<float>)>
    // public class OnMarkerMessageNode : GameObjectEventUnit<int>
    {
        private const string EventName = "OnMarkerMessage";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput intParams { get; private set; }

        [DoNotSerialize]
        public ValueOutput floatParams { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventName);
        }

        protected override void Definition()
        {
            base.Definition();
            intParams = ValueOutput<List<int>>(nameof(intParams));
            floatParams = ValueOutput<List<float>>(nameof(floatParams));

            // if (GlobalMessagePipe.IsInitialized)
            // {
            //     var subMarkerMessage = GlobalMessagePipe.GetSubscriber<CreatorMessages.MarkerMessage>();
            //     subMarkerMessage?
            //         .Subscribe(x =>
            //         {
            //             Debug.Log($"MarkerMessage: {x}");
            //             _canTrigger = true;
            //         })
            //         .AddTo(_compositeDisposable);
            // }

        }

        protected override bool ShouldTrigger(Flow flow, (List<int>, List<float>) args)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, (List<int>, List<float>) args)
        {
            flow.SetValue(intParams, args.Item1);
            flow.SetValue(floatParams, args.Item2);
        }
    }
}
