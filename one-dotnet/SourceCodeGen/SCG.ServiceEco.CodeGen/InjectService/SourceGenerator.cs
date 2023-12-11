using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TPFive.SCG.ServiceEco.CodeGen.InjectService
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
                    GeneratePartialClass(
                        candidate,
                        context.Compilation);

                context.AddSource(
                    fileName,
                    SourceText.From(sourceCode, Encoding.UTF8));
            }
        }

        private static (string fileName, string sourceCode) GeneratePartialClass(
            ClassDeclarationSyntax syntax,
            Compilation compilation)
        {
            var root = syntax.GetCompilationUnit();
            var classSemanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            var classSymbol = classSemanticModel.GetDeclaredSymbol(syntax);

            var doneServiceListFetching = false;
            var serviceList = string.Empty;
            var pubMessageList = string.Empty;
            var subMessageList = string.Empty;
            var theList = new List<string>();
            var doneSetupFetchig = false;
            var addConstructor = string.Empty;
            var setup = string.Empty;
            var declareSettings = string.Empty;
            var addLifetimeScope = string.Empty;

            foreach (var attributeData in classSymbol.GetAttributes())
            {
                theList.Add(attributeData.AttributeClass.Name);

                foreach (var namedArgument in attributeData.NamedArguments)
                {
                    theList.Add(namedArgument.Key);
                    if (namedArgument.Key == "ServiceList")
                    {
                        if (string.IsNullOrEmpty(serviceList))
                        {
                            serviceList = namedArgument.Value.Value.ToString() ?? string.Empty;
                            doneServiceListFetching = true;
                        }
                    }

                    if (namedArgument.Key == "PubMessageList")
                    {
                        if (string.IsNullOrEmpty(pubMessageList))
                        {
                            pubMessageList = namedArgument.Value.Value.ToString() ?? string.Empty;
                        }
                    }

                    if (namedArgument.Key == "SubMessageList")
                    {
                        if (string.IsNullOrEmpty(subMessageList))
                        {
                            subMessageList = namedArgument.Value.Value.ToString() ?? string.Empty;
                        }
                    }

                    if (namedArgument.Key == "AddConstructor")
                    {
                        if (string.IsNullOrEmpty(addConstructor))
                        {
                            addConstructor = namedArgument.Value.Value.ToString() ?? "false";
                        }
                    }

                    if (namedArgument.Key == "Setup")
                    {
                        if (string.IsNullOrEmpty(setup))
                        {
                            setup = namedArgument.Value.Value.ToString() ?? "false";
                            doneSetupFetchig = true;
                        }
                    }

                    if (namedArgument.Key == "DeclareSettings")
                    {
                        if (string.IsNullOrEmpty(declareSettings))
                        {
                            declareSettings = namedArgument.Value.Value.ToString() ?? "false";
                        }
                    }

                    if (namedArgument.Key == "AddLifetimeScope")
                    {
                        if (string.IsNullOrEmpty(addLifetimeScope))
                        {
                            addLifetimeScope = namedArgument.Value.Value.ToString() ?? "false";
                        }
                    }
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
                serviceList,
                pubMessageList,
                subMessageList,
                addConstructor,
                setup,
                declareSettings,
                addLifetimeScope,
                theList);

            return ($"{classModel.Name}.InjectService.Generated.cs", source);
        }
    }
}
