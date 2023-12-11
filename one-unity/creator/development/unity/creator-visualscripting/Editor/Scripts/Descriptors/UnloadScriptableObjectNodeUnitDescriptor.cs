using Unity.VisualScripting;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(UnloadScriptableObjectNode))]
    public class UnloadScriptableObjectNodeUnitDescriptor : UnitDescriptorBase<UnloadScriptableObjectNode>
    {
        public UnloadScriptableObjectNodeUnitDescriptor(UnloadScriptableObjectNode target) : base(target)
        {
        }
    }
}
