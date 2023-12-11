using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(UnloadSceneNode))]
    public class UnloadSceneNodeUnitDescriptor : UnitDescriptorBase<UnloadSceneNode>
    {
        public UnloadSceneNodeUnitDescriptor(UnloadSceneNode target) : base(target)
        {
        }
    }
}
