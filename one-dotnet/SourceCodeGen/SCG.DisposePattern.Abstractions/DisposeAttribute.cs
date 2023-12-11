using System;

namespace TPFive.SCG.DisposePattern.Abstractions
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DisposeAttribute : Attribute
    {
        public string DisposeHandler { get; set; } = "HandleDispose";
    }
}
