using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Creator.VisualScripting
{
    //
    using CrossBridge = TPFive.Cross.Bridge;
    using CreatorBridge = TPFive.Creator.Bridge;

    //
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    //
    [UnitTitle("XrSpace: Load Scene")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Load Scene")]
    [UnitCategory("XrSpace\\Actions")]
    // [TypeIcon(typeof(CreatorComponentBase))]
    public class LoadSceneNode : UnitBase
    {
        [DoNotSerialize] [PortLabelHidden] public ControlInput inputTrigger { get; private set; }

        [DoNotSerialize] [PortLabelHidden] public ControlOutput outputTrigger { get; private set; }

        //
        [DoNotSerialize] public ValueInput name { get; private set; }

        [DoNotSerialize] public ValueInput loadSceneMode { get; private set; }

        [DoNotSerialize] public ValueInput categoryOrder { get; private set; }
        [DoNotSerialize] public ValueInput subOrder { get; private set; }

        [DoNotSerialize] public ValueInput lifetimeScope { get; private set; }

        //
        [DoNotSerialize]
        public ValueOutput scene { get; private set; }

        protected override void Definition()
        {
            name = ValueInput<string>(nameof(name), string.Empty);
            loadSceneMode = ValueInput<LoadSceneMode>(nameof(loadSceneMode), LoadSceneMode.Additive);
            categoryOrder = ValueInput<int>(nameof(categoryOrder), 0);
            subOrder = ValueInput<int>(nameof(subOrder), 0);
            lifetimeScope = ValueInput<UnityEngine.MonoBehaviour>(nameof(lifetimeScope), null);

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            scene = ValueOutput<Scene>(nameof(scene), GetOutput);

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator Process(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(LoadSceneNode), 0, "Process");
            // var creator = CreatorBridge.GetCreator();
            //
            // if (creator == null)
            // {
            //     // return outputTrigger;
            //     yield return null;
            // }

            if (CrossBridge.LoadScene == null)
            {
                CrossBridge.Logging?.Invoke(typeof(LoadSceneNode), 0, "Don't have LoadScene");
                yield break;
            }

            yield return CrossBridge.LoadScene.Invoke(
                flow.GetValue<string>(name),
                flow.GetValue<LoadSceneMode>(loadSceneMode),
                flow.GetValue<int>(categoryOrder),
                flow.GetValue<int>(subOrder),
                flow.GetValue<UnityEngine.MonoBehaviour>(lifetimeScope)
                );

            flow.Run(outputTrigger);
            // flow.Invoke(outputTrigger);
            // flow.StartCoroutine(outputTrigger);
        }

        private Scene GetOutput(Flow flow)
        {
            if (CrossBridge.GetLoadedScene == null)
            {
                return default;
            }

            return CrossBridge.GetLoadedScene.Invoke(flow.GetValue<string>(name));
        }
    }
}
