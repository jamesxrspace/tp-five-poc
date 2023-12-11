using System;

namespace TPFive.SCG.ServiceEco.Abstractions
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RegisterToContainerAttribute : Attribute
    {
        public int Scope { get; set; } = 1;

        public string Category { get; set; } = "100";
    }
}
