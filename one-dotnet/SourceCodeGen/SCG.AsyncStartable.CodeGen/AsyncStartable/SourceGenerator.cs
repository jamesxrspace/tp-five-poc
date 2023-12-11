using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TPFive.SCG.AsyncStartable.CodeGen
{
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.Utility;

    /// <summary>
    /// Source generator will gather relevant data for actual code generation.
    /// </summary>
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        private const string SetupBeginMethod = "SetupBegin";
        private const string SetupEndMethod = "SetupEnd";

        // Make default to empty.
        private const string ExceptionList = "";

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

            var setupBeginMethod = GetSetupBeginValue(classSymbol);
            var setupEndMethod = GetSetupEndValue(classSymbol);
            var exceptionHandlerList = GetExceptionAndHandlerList(classSymbol);

            var properties = new List<PropertyData>();
            var classModel = new ClassModel(
                root.GetNamespace(),
                syntax.GetClassName(),
                syntax.GetClassModifier(),
                properties.ToArray());
            var source = CodeGenerator.Generate(
                classModel,
                setupBeginMethod,
                setupEndMethod,
                exceptionHandlerList);

            return ($"{classModel.Name}.StartAsync.Generated.cs", source);
        }

        private static string GetSetupBeginValue(ISymbol classSymbol)
        {
            var asyncStartableAttribute = classSymbol
                .GetAttributes()
                .FirstOrDefault(c => c.AttributeClass.Name == nameof(AsyncStartableAttribute));
            var setupBeginMethod = asyncStartableAttribute.GetAttributeValue(
                nameof(AsyncStartableAttribute.SetupBeginMethod), SetupBeginMethod);

            return setupBeginMethod;
        }

        private static string GetSetupEndValue(ISymbol classSymbol)
        {
            var asyncStartableAttribute = classSymbol
                .GetAttributes()
                .FirstOrDefault(c => c.AttributeClass.Name == nameof(AsyncStartableAttribute));
            var setupEndMethod = asyncStartableAttribute.GetAttributeValue(
                nameof(AsyncStartableAttribute.SetupEndMethod), SetupEndMethod);

            return setupEndMethod;
        }

        private static List<(string ExceptionType, string ExceptionHandler)> GetExceptionAndHandlerList(ISymbol classSymbol)
        {
            var asyncStartableAttribute = classSymbol
                .GetAttributes()
                .FirstOrDefault(c => c.AttributeClass.Name == nameof(AsyncStartableAttribute));

            var exceptionString = asyncStartableAttribute.GetAttributeValue(
                nameof(AsyncStartableAttribute.ExceptionList), ExceptionList);

            if (string.IsNullOrEmpty(exceptionString))
            {
                return new List<(string ExceptionType, string ExceptionHandler)>();
            }

            var exceptionTypes = exceptionString
                .Split(',')
                .Select(s => s.Trim())
                .ToList();
            var exceptionHandlers = exceptionString
                .Split(',')
                .Select(s => s.Trim().Split('.').Last())
                .ToList();

            var result = exceptionTypes
                .Zip(exceptionHandlers, (type, handler) => (type, handler))
                .ToList();

            return result;
        }
    }
}
