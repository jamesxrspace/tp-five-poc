using System;

namespace TPFive.SCG.AsyncStartable.Abstractions
{
    /// <summary>
    /// This is for the code generator to find the classes to generate code for.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AsyncStartableAttribute : Attribute
    {
        public string SetupBeginMethod { get; set; } = "SetupBegin";
        public string SetupEndMethod { get; set; } = "SetupEnd";

        public string ExceptionList { get; set; }
    }
}
