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
    [UnitTitle("XrSpace: Teleport Actor")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Teleport Actor")]
    [UnitCategory("XrSpace\\Actions")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class TeleportActorNode : UnitBase
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput gameObject { get; private set; }

        [DoNotSerialize]
        public ValueInput position { get; private set; }

        protected override void Definition()
        {
            gameObject = ValueInput<GameObject>(nameof(GameObject), null);
            position = ValueInput<Vector3>(nameof(Vector3), Vector3.zero);

            inputTrigger = ControlInput(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private ControlOutput Process(Flow flow)
        {
            // var creator = CreatorBridge.GetCreator();

            // if (creator == null)
            // {
            //     return outputTrigger;
            // }

            if (CrossBridge.TeleportActor == null)
            {
                CrossBridge.Logging?.Invoke(typeof(TeleportActorNode), 0, "Don't have TeleportActor");
                return outputTrigger;
            }

            CrossBridge.TeleportActor?.Invoke(
                flow.GetValue<GameObject>(gameObject),
                flow.GetValue<Vector3>(position));

            return outputTrigger;
        }
    }
}
