using System;

namespace TPFive.SCG.ServiceEco.Abstractions
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ServiceProviderManagementAttribute : Attribute
    {
        public string NullServiceProviderEnumName { get; set; }
    }
}
