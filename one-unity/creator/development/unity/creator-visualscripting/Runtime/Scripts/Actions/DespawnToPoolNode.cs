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
    [UnitTitle("XrSpace: Despawn to object pool")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Despawn to object pool")]
    [UnitCategory("XrSpace\\Actions")]
    // [TypeIcon(typeof(CreatorComponentBase))]
    public class DespawnToPoolNode : UnitBase
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
        public ValueInput gameObject { get; private set; }

        [DoNotSerialize]
        public ValueOutput result { get; private set; }

        private bool _result;

        protected override void Definition()
        {
            poolName = ValueInput<string>(nameof(poolName), null);
            gameObject = ValueInput<GameObject>(nameof(gameObject), null);

            inputTrigger = ControlInput(nameof(inputTrigger), Process);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            result = ValueOutput<bool>(nameof(result), GetOutput);

            Succession(inputTrigger, outputTrigger);
        }

        private ControlOutput Process(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(DespawnToPoolNode), 0, "Process");
            // var creator = CreatorBridge.GetCreator();

            // if (creator == null)
            // {
            //     return outputTrigger;
            // }

            if (CrossBridge.DespawnToPool == null)
            {
                CrossBridge.Logging?.Invoke(typeof(DespawnToPoolNode), 0, "Don't have Spawn");
                return outputTrigger;
            }

            _result = CrossBridge.DespawnToPool.Invoke(
                flow.GetValue<string>(poolName),
                flow.GetValue<GameObject>(gameObject));

            flow.SetValue(result, _result);

            return outputTrigger;
        }

        private bool GetOutput(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(DespawnToPoolNode), 0, "GetOutput");

            // if (CrossBridge.SpawnFromPool == null)
            // {
            //     return default;
            // }
            //
            // return CrossBridge.SpawnFromPool.Invoke(
            //     flow.GetValue<string>(poolName),
            //     flow.GetValue<GameObject>(prefab));

            return _result;
        }
    }
}
