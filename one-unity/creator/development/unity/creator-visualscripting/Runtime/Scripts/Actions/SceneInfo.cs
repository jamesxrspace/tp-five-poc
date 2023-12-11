using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting
{
    using CrossBridge = TPFive.Cross.Bridge;
    using CreatorBridge = TPFive.Creator.Bridge;
    using CreatorComponentBase = TPFive.Creator.ComponentBase;

    [UnitTitle("XrSpace: Scene Info")]
    [UnitSurtitle("XrSpace")]
    [UnitShortTitle("Scene Info")]
    [UnitCategory("XrSpace\\Actions")]
    public class SceneInfo : Unit
    {
        [DoNotSerialize] public ValueInput Title { get; private set; }
        [DoNotSerialize] public ValueInput Category { get; private set; }
        [DoNotSerialize] public ValueInput CategoryOrder { get; private set; }
        [DoNotSerialize] public ValueInput SubOrder { get; private set; }
        [DoNotSerialize] public ValueOutput sceneInfo { get; private set; }

        protected override void Definition()
        {
            Title = ValueInput(nameof(Title), string.Empty);
            Category = ValueInput(nameof(Category), string.Empty);
            CategoryOrder = ValueInput(nameof(CategoryOrder), 0);
            SubOrder = ValueInput(nameof(SubOrder), 0);
            sceneInfo = ValueOutput<SceneInfo>(nameof(sceneInfo), flow => this);
        }
    }
}