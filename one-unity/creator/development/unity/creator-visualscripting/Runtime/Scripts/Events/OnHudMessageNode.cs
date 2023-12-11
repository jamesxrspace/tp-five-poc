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

    [UnitTitle("On Hud Message")]
    [UnitCategory("Events\\XrSpace")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class OnHudMessageNode : EventUnitBase<(List<int>, List<float>, List<string>, List<GameObject>)>
    // public class OnMarkerMessageNode : GameObjectEventUnit<int>
    {
        private const string EventName = "OnHudMessage";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput intParams { get; private set; }

        [DoNotSerialize]
        public ValueOutput floatParams { get; private set; }

        [DoNotSerialize]
        public ValueOutput stringParams { get; private set; }

        [DoNotSerialize]
        public ValueOutput gameObjectParams { get; private set; }


        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventName);
        }

        protected override void Definition()
        {
            base.Definition();
            intParams = ValueOutput<List<int>>(nameof(intParams));
            floatParams = ValueOutput<List<float>>(nameof(floatParams));
            stringParams = ValueOutput<List<string>>(nameof(stringParams));
            gameObjectParams = ValueOutput<List<GameObject>>(nameof(gameObjectParams));

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

        protected override bool ShouldTrigger(Flow flow, (List<int>, List<float>, List<string>, List<GameObject>) args)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, (List<int>, List<float>, List<string>, List<GameObject>) args)
        {
            flow.SetValue(intParams, args.Item1);
            flow.SetValue(floatParams, args.Item2);
            flow.SetValue(stringParams, args.Item3);
            flow.SetValue(gameObjectParams, args.Item4);
        }
    }
}
