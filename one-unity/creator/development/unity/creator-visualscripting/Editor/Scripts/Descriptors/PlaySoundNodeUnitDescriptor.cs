using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(PlaySoundNode))]
    public class PlaySoundNodeUnitDescriptor : UnitDescriptorBase<PlaySoundNode>
    {
        public PlaySoundNodeUnitDescriptor(PlaySoundNode target) : base(target)
        {
        }
    }
}
