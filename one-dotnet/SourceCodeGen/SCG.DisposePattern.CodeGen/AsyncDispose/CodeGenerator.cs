﻿namespace TPFive.SCG.DisposePattern.CodeGen.AsyncDispose
{
    /// <summary>
    /// Actual code generation.
    /// </summary>
    internal static class CodeGenerator
    {
        public static string Generate(
            ClassModel model,
            string asyncDisposeHandlerName,
            bool hasDispose,
            string disposeHandlerName)
        {
            var callDispose = hasDispose ? $"{disposeHandlerName}(disposing: false);" : "// No dispose method";
            var output = $@"
// <auto-generated />

using System.Threading;
using System.Threading.Tasks;

namespace {model.Namespace}
{{
    /// <summary>
    /// This part deals dispose async. It works with DisposeAttribute.
    /// </summary>
    {model.Modifier} class {model.Name} :
        System.IAsyncDisposable
    {{
        public async ValueTask DisposeAsync()
        {{
            await {asyncDisposeHandlerName}().ConfigureAwait(true);
            {callDispose}
            System.GC.SuppressFinalize(this);
        }}
    }}
}}
";

            return output;
        }
    }
}
