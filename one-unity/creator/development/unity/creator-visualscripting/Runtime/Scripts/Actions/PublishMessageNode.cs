using Unity.VisualScripting;
using UnityEngine;

namespace TPFive.Creator.VisualScripting
{
    //
    using CrossBridge = TPFive.Cross.Bridge;

    //
    using CreatorComponentBase = TPFive.Creator.ComponentBase;
    using CreatorBridge = TPFive.Creator.Bridge;

    //
    [UnitTitle("XrSpace: Publish Message")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Publish Message")]
    [UnitCategory("XrSpace\\Actions")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class PublishMessageNode : UnitBase
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput name { get; private set; }

        // For current use one string param is sufficient, will add more if necessary
        [DoNotSerialize]
        public ValueInput stringParam { get; private set; }

        protected override void Definition()
        {
            name = ValueInput<string>(nameof(name), string.Empty);
            stringParam = ValueInput<string>(nameof(stringParam), string.Empty);

            inputTrigger = ControlInput(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private ControlOutput Process(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(PublishMessageNode), 0, "Process");

            if (CrossBridge.PublishMessage == null)
            {
                CrossBridge.Logging?.Invoke(typeof(PublishMessageNode), 0, "Don't have PublishMessage");
                return outputTrigger;
            }

            CrossBridge.PublishMessage?.Invoke(
                flow.GetValue<string>(name),
                flow.GetValue<string>(stringParam));

            return outputTrigger;
        }
    }
}
