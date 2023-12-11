using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TPFive.SCG.ServiceEco.CodeGen.ServiceProviderManagement
{
    using TPFive.SCG.ServiceEco.Abstractions;
    using TPFive.SCG.Utility;

    /// <summary>
    /// Source generator will gather relevant data for actual code generation.
    /// </summary>
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        private const string NullServiceProviderEnumName = "TPFive.Game.ServiceProviderKind.NullServiceProvider";

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
            SemanticModel classSemanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            var classSymbol = classSemanticModel.GetDeclaredSymbol(syntax);

            var fullEnumName = GetFullEnumNameValue(classSymbol);

            var properties = new List<PropertyData>();
            var classModel = new ClassModel(
                root.GetNamespace(),
                syntax.GetClassName(),
                syntax.GetClassModifier(),
                properties.ToArray());
            var source = CodeGenerator.Generate(
                classModel,
                fullEnumName);

            return ($"{classModel.Name}.ServiceProviderManagement.Generated.cs", source);
        }

        private static string GetFullEnumNameValue(ISymbol classSymbol)
        {
            var attribute = classSymbol
                .GetAttributes()
                .FirstOrDefault(c => c.AttributeClass.Name == nameof(ServiceProviderManagementAttribute));
            var fullEnumName = attribute.GetAttributeValue(
                nameof(ServiceProviderManagementAttribute.NullServiceProviderEnumName), NullServiceProviderEnumName);

            return fullEnumName;
        }
    }
}
