using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting
{
    //
    using CrossBridge = TPFive.Cross.Bridge;
    using CreatorBridge = TPFive.Creator.Bridge;

    //
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    //
    [UnitTitle("XrSpace: Show Voice Message Board Window")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Show Voice Message Board Window")]
    [UnitCategory("XrSpace\\Actions")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class ShowVoiceMessageBoardWindowNode : UnitBase
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput name { get; private set; }

        protected override void Definition()
        {
            name = ValueInput<string>(nameof(name), string.Empty);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) =>
            {
                CrossBridge.ShowVoiceMessageBoardWindow?.Invoke(
                    f.GetValue<string>(name));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private ControlOutput Process(Flow flow)
        {
            var creator = CreatorBridge.GetCreator();

            if (creator == null)
            {
                return outputTrigger;
            }

            return outputTrigger;
        }
    }
}
