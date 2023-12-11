using Unity.VisualScripting;
using UnityEngine;

namespace TPFive.Creator.VisualScripting
{
    //
    using CrossBridge = TPFive.Cross.Bridge;
    using CreatorBridge = TPFive.Creator.Bridge;

    //
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    //
    [UnitTitle("XrSpace: Play Sound")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Play Sound")]
    [UnitCategory("XrSpace\\Actions")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class PlaySoundNode : UnitBase
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

            inputTrigger = ControlInput(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private ControlOutput Process(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(PlaySoundNode), 0, "Process");
            // var creator = CreatorBridge.GetCreator();

            // if (creator == null)
            // {
            //     return outputTrigger;
            // }

            if (CrossBridge.PlaySound == null)
            {
                CrossBridge.Logging?.Invoke(typeof(PlaySoundNode), 0, "Don't have PlaySound");
                return outputTrigger;
            }

            CrossBridge.PlaySound?.Invoke(
                flow.GetValue<string>(name));

            return outputTrigger;
        }
    }
}
