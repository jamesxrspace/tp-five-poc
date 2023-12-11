using System;

namespace TPFive.SCG.DisposePattern.Abstractions
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AsyncDisposeAttribute : Attribute
    {
        public string AsyncDisposeHandler { get; set; } = "HandleDisposeAsync";
    }
}
