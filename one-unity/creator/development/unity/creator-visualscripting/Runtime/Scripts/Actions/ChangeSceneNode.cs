using Unity.VisualScripting;
using UnityEngine;

namespace TPFive.Creator.VisualScripting
{
    using CrossBridge = TPFive.Cross.Bridge;
    using CreatorBridge = TPFive.Creator.Bridge;
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    [UnitTitle("XrSpace: Change Scene")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Change Scene")]
    [UnitCategory("XrSpace\\Actions")]
    public class ChangeSceneNode : UnitBase
    {
        [DoNotSerialize] [PortLabelHidden] public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize] public ValueInput fromSceneInfo { get; private set; }
        [DoNotSerialize] public ValueInput toSceneInfo { get; private set; }
        [DoNotSerialize] public ValueInput lifetimeScope { get; private set; }

        protected override void Definition()
        {
            fromSceneInfo = ValueInput<SceneInfo>(nameof(fromSceneInfo), null);
            toSceneInfo = ValueInput<SceneInfo>(nameof(toSceneInfo), null);
            lifetimeScope = ValueInput<MonoBehaviour>(nameof(lifetimeScope), null);
            inputTrigger = ControlInput(nameof(inputTrigger), Process);
        }

        private ControlOutput Process(Flow flow)
        {
            CrossBridge.Logging?.Invoke(typeof(ChangeSceneNode), 0, "Process");

            if (CrossBridge.ChangeScene == null)
            {
                CrossBridge.Logging?.Invoke(typeof(ChangeSceneNode), 0, "CrossBridge.ChangeScene is null");
                return null;
            }

            var from = flow.GetValue<SceneInfo>(this.fromSceneInfo);
            var to = flow.GetValue<SceneInfo>(this.toSceneInfo);

            var fromCategory = flow.GetValue<string>(from.Category);
            var fromTitleKey = flow.GetValue<string>(from.Title) + ".asset";
            var fromCategoryOrder = flow.GetValue<int>(from.CategoryOrder);
            var fromSubOrder = flow.GetValue<int>(from.SubOrder);

            var toCategory = flow.GetValue<string>(to.Category);
            var toTitleKey = flow.GetValue<string>(to.Title) + ".asset";
            var toCategoryOrder = flow.GetValue<int>(to.CategoryOrder);
            var toSubOrder = flow.GetValue<int>(to.SubOrder);

            var lifetimeScope = flow.GetValue<MonoBehaviour>(this.lifetimeScope);

            CrossBridge.ChangeScene.Invoke(
                fromCategory,
                fromTitleKey,
                fromCategoryOrder,
                fromSubOrder,
                toCategory,
                toTitleKey,
                toCategoryOrder,
                toSubOrder,
                lifetimeScope
            );

            return null;
        }
    }
}