using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(LoadScriptableObjectNode))]
    public class LoadScriptableObjectNodeUnitDescriptor : UnitDescriptorBase<LoadScriptableObjectNode>
    {
        public LoadScriptableObjectNodeUnitDescriptor(LoadScriptableObjectNode target) : base(target)
        {
        }
    }
}
