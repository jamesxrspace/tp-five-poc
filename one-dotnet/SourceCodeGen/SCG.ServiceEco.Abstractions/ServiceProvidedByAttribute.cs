using System;

namespace TPFive.SCG.ServiceEco.Abstractions
{
    /// <summary>
    /// Will create a null service provider.
    /// This should co-work with ServiceProviderManagementAttribute. As the null service provider
    /// should be as the default provider to the service.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ServiceProvidedByAttribute : Attribute
    {
        public ServiceProvidedByAttribute(System.Type providerType)
        {
            ProviderType = providerType;
        }

        public System.Type ProviderType { get; set; }

    }
}
