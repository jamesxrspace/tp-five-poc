using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(LoadGameObjectNode))]
    public class LoadGameObjectNodeUnitDescriptor : UnitDescriptorBase<LoadGameObjectNode>
    {
        public LoadGameObjectNodeUnitDescriptor(LoadGameObjectNode target) : base(target)
        {
        }
    }
}
