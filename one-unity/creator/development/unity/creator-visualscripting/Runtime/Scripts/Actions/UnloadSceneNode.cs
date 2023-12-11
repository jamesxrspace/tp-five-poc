using System.Collections;
using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting
{
    //
    using CrossBridge = TPFive.Cross.Bridge;
    using CreatorBridge = TPFive.Creator.Bridge;

    //
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    //
    [UnitTitle("XrSpace: Unload Scene")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Unload Scene")]
    [UnitCategory("XrSpace\\Actions")]
    // [TypeIcon(typeof(CreatorComponentBase))]
    public class UnloadSceneNode : UnitBase
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput name { get; private set; }

        [DoNotSerialize]
        public ValueInput categoryOrder { get; private set; }

        [DoNotSerialize]
        public ValueInput subOrder { get; private set; }

        protected override void Definition()
        {
            name = ValueInput<string>(nameof(name), string.Empty);
            categoryOrder = ValueInput<int>(nameof(categoryOrder), 0);
            subOrder = ValueInput<int>(nameof(subOrder), 0);

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator Process(Flow flow)
        {
            yield return CrossBridge.UnloadScene.Invoke(
                flow.GetValue<string>(name),
                flow.GetValue<int>(categoryOrder),
                flow.GetValue<int>(subOrder));

            flow.Run(outputTrigger);
        }
    }
}
