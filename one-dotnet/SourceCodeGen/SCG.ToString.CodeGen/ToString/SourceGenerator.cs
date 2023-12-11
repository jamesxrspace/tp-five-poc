using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TPFive.SCG.ToString.CodeGen
{
    using TPFive.SCG.ToString.Abstractions;
    using TPFive.SCG.Utility;

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

            var displayCollections = GetDisplayCollectionsValue(classSymbol);
            var propertyTransformer = new PropertyTransformer(
                classSemanticModel.Compilation,
                displayCollections);
            var properties = classSymbol.GetProperties()
                .Select(propertyTransformer.Transform);
            var classModel = new ClassModel(
                root.GetNamespace(),
                syntax.GetClassName(),
                syntax.GetClassModifier(),
                properties.ToArray());
            var source = CodeGenerator.Generate(
                classModel);

            return ($"{classModel.Name}.ToString.cs", source);
        }

        private static bool GetDisplayCollectionsValue(INamedTypeSymbol classSymbol)
        {
            var toStringAttribute = classSymbol
                .GetAttributes()
                .FirstOrDefault(c => c.AttributeClass.Name == nameof(ToStringAttribute));
            var displayCollections = toStringAttribute
                .GetAttributeValue(nameof(ToStringAttribute.DisplayCollections), false);
            return displayCollections;
        }
    }
}
