using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TPFive.SCG.DisposePattern.CodeGen.AsyncDispose
{
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.Utility;

    /// <summary>
    /// Source generator will gather relevant data for actual code generation.
    /// </summary>
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        private const string HandleDisposeAsync = "HandleDisposeAsync";
        private const string HandleDispose = "HandleDispose";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver) return;

            foreach (var candidate in syntaxReceiver.Candidates)
            {
                var (fileName, sourceCode) =
                    GeneratePartialClass(candidate, context.Compilation);

                context.AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));
            }
        }

        private static (string FileName, string SourceCode) GeneratePartialClass(
            ClassDeclarationSyntax syntax,
            Compilation compilation)
        {
            var root = syntax.GetCompilationUnit();
            var classSemanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            var classSymbol = classSemanticModel.GetDeclaredSymbol(syntax);

            var asyncDisposeHandler = GetAsyncDisposeHandlerValue(classSymbol);
            var (hasDispose, disposeHandler) = GetDisposeHandlerValue(classSymbol);

            var properties = new List<PropertyData>();
            var classModel = new ClassModel(
                root.GetNamespace(),
                syntax.GetClassName(),
                syntax.GetClassModifier(),
                properties.ToArray());
            var source = CodeGenerator.Generate(
                classModel,
                asyncDisposeHandler,
                hasDispose,
                disposeHandler);

            return ($"{classModel.Name}.AsyncDispose.Generated.cs", source);
        }

        private static string GetAsyncDisposeHandlerValue(ISymbol classSymbol)
        {
            var disposeAttribute = classSymbol
                .GetAttributes()
                .FirstOrDefault(c => nameof(AsyncDisposeAttribute).StartsWith(c.AttributeClass.Name));
            var asyncDisposeHandler = disposeAttribute.GetAttributeValue(
                nameof(AsyncDisposeAttribute.AsyncDisposeHandler), HandleDisposeAsync);

            return asyncDisposeHandler;
        }

        private static (bool HasHandler, string Handler) GetDisposeHandlerValue(ISymbol classSymbol)
        {
            var disposeAttribute = classSymbol
                .GetAttributes()
                .FirstOrDefault(c => nameof(DisposeAttribute).StartsWith(c.AttributeClass.Name));
            if (disposeAttribute is null)
            {
                return (default, default);
            }

            var disposeHandler = disposeAttribute.GetAttributeValue(
                nameof(DisposeAttribute.DisposeHandler), HandleDispose);

            return (true, disposeHandler);
        }
    }
}
