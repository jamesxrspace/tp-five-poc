using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting
{
    //
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    [UnitTitle("On Actor Joined")]
    [UnitCategory("Events\\XrSpace")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class OnActorJoinedNode : EventUnitBase<int>
    {
        private const string EventName = "OnActorJoined";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput actor { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventName);
        }

        protected override void Definition()
        {
            base.Definition();
            actor = ValueOutput<int>(nameof(actor));
        }

        protected override bool ShouldTrigger(Flow flow, int args)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, int args)
        {
            flow.SetValue(actor, args);
        }
    }
}
