using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TPFive.SCG.Logging.CodeGen.EditorUse
{
    using TPFive.SCG.Logging.Abstractions;
    using TPFive.SCG.Utility;

    /// <summary>
    /// Source generator will gather relevant data for actual code generation.
    /// </summary>
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
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

        private static (string fileName, string sourceCode) GeneratePartialClass(
            ClassDeclarationSyntax syntax,
            Compilation compilation)
        {
            var root = syntax.GetCompilationUnit();
            var classSemanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            var classSymbol = classSemanticModel.GetDeclaredSymbol(syntax);

            var properties = new List<PropertyData>();
            var classModel = new ClassModel(
                root.GetNamespace(),
                syntax.GetClassName(),
                syntax.GetClassModifier(),
                properties.ToArray());
            var source = CodeGenerator.Generate(classModel);

            return ($"{classModel.Name}.EditorLogging.Generated.cs", source);
        }
    }

}
