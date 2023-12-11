using System;

namespace TPFive.SCG.ServiceEco.Abstractions
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RegisterServiceAttribute : Attribute
    {
        public RegisterServiceAttribute(string someName = "ray", int age = 20)
        {
            this.someName = someName;
            this.age = age;
        }

        public string someName = "sdfds";
        public int age = 10;

        public int Hp { get; set; }

        // public string Category { get; set; } = "100";
        public string ServiceList { get; set; } = "";
    }
}
