using System;
using System.Reflection;

namespace CSharpDataModel
{
    public static class CSharpModelAssembly
    {
        public static readonly Assembly Value = typeof(CSharpModelAssembly).Assembly;
        public static readonly Type IgnoreType = typeof(CSharpModelAssembly);
    }
}
