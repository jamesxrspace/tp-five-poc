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
    [UnitTitle("XrSpace: Get Loaded GameObject")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Get Loaded GameObject")]
    [UnitCategory("XrSpace\\Actions")]
    [TypeIcon(typeof(CreatorComponentBase))]
    public sealed class GetLoadedGameObjectNode : UnitBase
    {
        [DoNotSerialize]
        public ValueInput name { get; private set; }

        [DoNotSerialize]
        // [Inspectable]
        public ValueOutput gameObject { get; private set; }

        // private GameObject _gameObject;

        protected override void Definition()
        {
            name = ValueInput<string>(nameof(name), string.Empty);

            gameObject = ValueOutput<GameObject>(nameof(gameObject), GetOutput);
        }

        private GameObject GetOutput(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(GetLoadedGameObjectNode), 0, "GetOutput");

            if (CrossBridge.GetLoadedGameObject == null)
            {
                return default;
            }

            return CrossBridge.GetLoadedGameObject.Invoke(flow.GetValue<string>(name));
        }
    }
}
