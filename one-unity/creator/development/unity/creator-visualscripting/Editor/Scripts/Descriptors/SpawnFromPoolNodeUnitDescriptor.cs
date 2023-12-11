using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(SpawnFromPoolNode))]
    public class SpawnFromPoolNodeUnitDescriptor : UnitDescriptorBase<SpawnFromPoolNode>
    {
        public SpawnFromPoolNodeUnitDescriptor(SpawnFromPoolNode target) : base(target)
        {
        }
    }
}
