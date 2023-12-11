using System;

namespace TPFive.SCG.ToString.Abstractions
{
    [System.AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ToStringAttribute : Attribute
    {
        public bool DisplayCollections { get; set; } = false;
    }
}
