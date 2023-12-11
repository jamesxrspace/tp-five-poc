using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(DespawnToPoolNode))]
    public class DespawnToPoolNodeUnitDescriptor : UnitDescriptorBase<DespawnToPoolNode>
    {
        public DespawnToPoolNodeUnitDescriptor(DespawnToPoolNode target) : base(target)
        {
        }
    }
}
