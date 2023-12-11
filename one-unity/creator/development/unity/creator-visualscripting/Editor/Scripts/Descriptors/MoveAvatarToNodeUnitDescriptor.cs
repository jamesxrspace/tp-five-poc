using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(MoveAvatarToNode))]
    public class MoveAvatarToNodeUnitDescriptor : UnitDescriptorBase<MoveAvatarToNode>
    {
        public MoveAvatarToNodeUnitDescriptor(MoveAvatarToNode target) : base(target)
        {
        }
    }
}
