using System.Collections;
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
    [UnitTitle("XrSpace: Load ScriptableObject")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Load ScriptableObject")]
    [UnitCategory("XrSpace\\Actions")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public class LoadScriptableObjectNode : UnitBase
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
        public ValueOutput scriptableObject { get; private set; }

        protected override void Definition()
        {
            name = ValueInput<string>(nameof(name), string.Empty);

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            scriptableObject = ValueOutput<ScriptableObject>(nameof(scriptableObject), GetOutput);

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator Process(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(LoadScriptableObjectNode), 0, "Process");
            // var creator = CreatorBridge.GetCreator();

            // if (creator == null)
            // {
            //     // return outputTrigger;
            //     yield return null;
            // }

            if (CrossBridge.LoadScriptableObject == null)
            {
                yield return null;
            }

            yield return CrossBridge.LoadScriptableObject.Invoke(
                flow.GetValue<string>(name));

            flow.Run(outputTrigger);
        }


        private ScriptableObject GetOutput(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(LoadScriptableObjectNode), 0, "GetOutput");

            if (CrossBridge.GetLoadedScriptableObject == null)
            {
                return default;
            }

            return CrossBridge.GetLoadedScriptableObject.Invoke(flow.GetValue<string>(name));
        }
    }
}
