using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(LoadSceneNode))]
    public class LoadSceneNodeUnitDescriptor : UnitDescriptorBase<LoadSceneNode>
    {
        public LoadSceneNodeUnitDescriptor(LoadSceneNode target) : base(target)
        {
        }
    }
}
