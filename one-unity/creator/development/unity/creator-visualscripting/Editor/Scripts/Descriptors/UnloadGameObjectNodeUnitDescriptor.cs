using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(UnloadGameObjectNode))]
    public class UnloadGameObjectNodeUnitDescriptor : UnitDescriptorBase<UnloadGameObjectNode>
    {
        public UnloadGameObjectNodeUnitDescriptor(UnloadGameObjectNode target) : base(target)
        {
        }
    }
}
