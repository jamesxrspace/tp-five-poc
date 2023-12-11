using System.Collections;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace TPFive.Creator.VisualScripting
{
    using CrossBridge = TPFive.Cross.Bridge;
    using CreatorBridge = TPFive.Creator.Bridge;
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    [UnitTitle("XrSpace: Load Bundle")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Load Bundle")]
    [UnitCategory("XrSpace\\Actions")]
    public class LoadBundleNode : UnitBase
    {
        [DoNotSerialize] public ValueInput key { get; private set; }
        [DoNotSerialize] [PortLabelHidden] public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize] [PortLabelHidden] public ControlOutput outputTrigger { get; private set; }

        protected override void Definition()
        {
            key = ValueInput<string>(nameof(key), string.Empty);
            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), Process);
            outputTrigger = ControlOutput(nameof(outputTrigger));
        }

        private IEnumerator Process(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(LoadSceneNode), 0, "Process");

            if (CrossBridge.LoadBundledDataAsync == null)
            {
                CrossBridge.Logging?.Invoke(typeof(LoadSceneNode), 0, "LoadBundled fail : There is no LoadBundledDataAsync can be invoked.");

                yield break;
            }

            var task = CrossBridge.LoadBundledDataAsync.Invoke(
                flow.GetValue<string>(key), default
            ).AsTask();

            yield return new WaitUntil(() => task.IsCompleted);

            flow.Run(outputTrigger);
        }
    }
}