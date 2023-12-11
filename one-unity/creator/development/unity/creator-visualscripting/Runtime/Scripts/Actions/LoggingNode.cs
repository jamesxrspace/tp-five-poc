using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting
{
    //
    using CrossBridge = TPFive.Cross.Bridge;
    using CreatorBridge = TPFive.Creator.Bridge;

    //
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    //
    [UnitTitle("XrSpace: Logging")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Logging")]
    [UnitCategory("XrSpace\\Actions")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class LoggingNode : UnitBase
    {
        [Serialize, Inspectable, UnitHeaderInspectable("Level")]
        public LoggingLevel Level { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        // [DoNotSerialize]
        // public ValueInput level { get; private set; }

        [DoNotSerialize]
        public ValueInput message { get; private set; }

        protected override void Definition()
        {
            // level = ValueInput<int>(nameof(level), 0);
            message = ValueInput<object>(nameof(message), string.Empty);

            inputTrigger = ControlInput(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private ControlOutput Process(Flow flow)
        {
            // var creator = CreatorBridge.GetCreator();
            //
            // if (creator == null)
            // {
            //     return outputTrigger;
            // }

            CrossBridge.Logging?.Invoke(
                // f.GetValue<int>(level),
                typeof(LoggingNode),
                (int)Level,
                flow.GetValue(message));

            return outputTrigger;
        }

        public enum LoggingLevel
        {
            EditorDebug,
            Debug,
            Information
        }
    }
}
