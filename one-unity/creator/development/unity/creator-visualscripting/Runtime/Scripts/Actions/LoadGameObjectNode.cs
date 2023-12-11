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
    [UnitTitle("XrSpace: Load GameObject")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Load GameObject")]
    [UnitCategory("XrSpace\\Actions")]
    // [TypeIcon(typeof(CreatorComponentBase))]
    public class LoadGameObjectNode : UnitBase
    // public class LoadGameObjectNode : Unit
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
        public ValueOutput gameObject { get; private set; }

        // private GameObject _gameObject;

        protected override void Definition()
        {
            name = ValueInput<string>(nameof(name), string.Empty);

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            gameObject = ValueOutput<GameObject>(nameof(gameObject), GetOutput);

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator Process(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(LoadGameObjectNode), 0, "Process");
            // var creator = CreatorBridge.GetCreator();
            //
            // if (creator == null)
            // {
            //     // return outputTrigger;
            //     yield return null;
            // }

            if (CrossBridge.LoadGameObject == null)
            {
                CrossBridge.Logging?.Invoke(typeof(LoadGameObjectNode), 0, "Don't have LoadGameObject");
                yield break;
            }

            yield return CrossBridge.LoadGameObject.Invoke(
                flow.GetValue<string>(name));

            flow.Run(outputTrigger);
            // flow.StartCoroutine(outputTrigger);
        }

        private GameObject GetOutput(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(LoadGameObjectNode), 0, "GetOutput");

            if (CrossBridge.GetLoadedGameObject == null)
            {
                return default;
            }

            return CrossBridge.GetLoadedGameObject.Invoke(flow.GetValue<string>(name));
        }
    }
}
