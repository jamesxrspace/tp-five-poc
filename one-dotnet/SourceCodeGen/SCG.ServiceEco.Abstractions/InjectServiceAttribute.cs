using System;

namespace TPFive.SCG.ServiceEco.Abstractions
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class InjectServiceAttribute : Attribute
    {
        public InjectServiceAttribute()
        {
        }

        public string Category { get; set; } = "";

        public string AddConstructor { get; set; } = "";
        public string Setup { get; set; } = "";
        public string DeclareSettings { get; set; } = "";

        public string AddLifetimeScope { get; set; } = "";
        // public string UseLogger { get; set; } = "";
        public string ServiceList { get; set; } = "";
        public string PubMessageList { get; set; } = "";
        public string SubMessageList { get; set; } = "";
    }
}
