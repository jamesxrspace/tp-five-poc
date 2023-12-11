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
    [UnitTitle("XrSpace: Spawn from object pool")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Spawn from object pool")]
    [UnitCategory("XrSpace\\Actions")]
    // [TypeIcon(typeof(CreatorComponentBase))]
    public class SpawnFromPoolNode : UnitBase
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput poolName { get; private set; }

        [DoNotSerialize]
        public ValueInput prefab { get; private set; }

        [DoNotSerialize]
        public ValueOutput gameObject { get; private set; }

        private GameObject _resultGO;

        protected override void Definition()
        {
            poolName = ValueInput<string>(nameof(poolName), null);
            prefab = ValueInput<GameObject>(nameof(prefab), null);

            inputTrigger = ControlInput(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            gameObject = ValueOutput<GameObject>(nameof(gameObject), GetOutput);

            Succession(inputTrigger, outputTrigger);
        }

        private ControlOutput Process(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(SpawnFromPoolNode), 0, "Process");
            // var creator = CreatorBridge.GetCreator();

            // if (creator == null)
            // {
            //     return outputTrigger;
            // }

            if (CrossBridge.SpawnFromPool == null)
            {
                CrossBridge.Logging?.Invoke(typeof(SpawnFromPoolNode), 0, "Don't have Spawn");
                return outputTrigger;
            }

            _resultGO = CrossBridge.SpawnFromPool.Invoke(
                flow.GetValue<string>(poolName),
                flow.GetValue<GameObject>(prefab));

            flow.SetValue(gameObject, _resultGO);

            return outputTrigger;
        }

        private GameObject GetOutput(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(SpawnFromPoolNode), 0, "GetOutput");

            // if (CrossBridge.SpawnFromPool == null)
            // {
            //     return default;
            // }
            //
            // return CrossBridge.SpawnFromPool.Invoke(
            //     flow.GetValue<string>(poolName),
            //     flow.GetValue<GameObject>(prefab));

            return _resultGO;
        }
    }
}
