using System;

namespace TPFive.SCG.Bridge.Abstractions
{
    [System.AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DelegateFromAttribute : Attribute
    {
        public string DelegateName { get; set; } = "DelegateName";
    }
}