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
    [UnitTitle("XrSpace: Unload ScriptableObject")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Unload ScriptableObject")]
    [UnitCategory("XrSpace\\Actions")]
    // [TypeIcon(typeof(CreatorComponentBase))]
    public class UnloadScriptableObjectNode : UnitBase
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

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator Process(Flow flow)
        {
            var creator = CreatorBridge.GetCreator();

            if (creator == null)
            {
                // return outputTrigger;
                yield return null;
            }

            if (CrossBridge.UnloadScriptableObject == null)
            {
                yield return null;
            }

            yield return CrossBridge.UnloadScriptableObject.Invoke(
                flow.GetValue<string>(name));
        }
    }
}
