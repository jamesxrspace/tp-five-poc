using System;

namespace TPFive.SCG.Logging.Abstractions
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]

    public class EditorUseLoggingAttribute : Attribute
    {
    }
}
