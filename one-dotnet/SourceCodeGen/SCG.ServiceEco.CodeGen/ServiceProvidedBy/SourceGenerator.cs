using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TPFive.SCG.ServiceEco.CodeGen.ServiceProvidedBy
{
    using TPFive.SCG.ServiceEco.Abstractions;
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
                    GeneratePartialClass(candidate, context);

                context.AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));
            }
        }

        private (string fileName, string sourceCode) GeneratePartialClass(
            ClassDeclarationSyntax syntax,
            GeneratorExecutionContext context)
        {
            var root = syntax.GetCompilationUnit();
            var classSemanticModel = context.Compilation.GetSemanticModel(syntax.SyntaxTree);
            var classSymbol = classSemanticModel.GetDeclaredSymbol(syntax);

            var specificAttribute = classSymbol
                .GetAttributes()
                .FirstOrDefault(c => c.AttributeClass.Name == nameof(ServiceProvidedByAttribute));

            var interfaceType = specificAttribute
                .GetAttributeValue<System.Type>(nameof(ServiceProvidedByAttribute.ProviderType));

            List<(INamedTypeSymbol, InterfaceDeclarationSyntax)> tempMethodDeclarations = new();

            // Get all syntax trees in the compilation
            var syntaxTrees = context.Compilation.SyntaxTrees;

            // Loop through all syntax trees
            foreach (var syntaxTree in syntaxTrees)
            {
                // Get the root node of the syntax tree
                var rootNode = syntaxTree.GetRoot();
                // Find all interface declarations in the syntax tree
                var interfaceDeclarations =
                    rootNode.DescendantNodes().OfType<InterfaceDeclarationSyntax>();

                // Loop through all interface declarations
                foreach (var interfaceDeclaration in interfaceDeclarations)
                {
                    // Get the type symbol of the interface
                    var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
                    var interfaceSymbol = semanticModel.GetDeclaredSymbol(interfaceDeclaration);

                    tempMethodDeclarations.Add((interfaceSymbol, interfaceDeclaration));
                }
            }

            var properties = new List<PropertyData>();
            var classModel = new ClassModel(
                root.GetNamespace(),
                syntax.GetClassName(),
                syntax.GetClassModifier(),
                properties.ToArray());
            var source = CodeGenerator.Generate(
                classModel,
                tempMethodDeclarations,
                "");

            return ($"NullServiceProvider.Generated.cs", source);
        }
    }
}
