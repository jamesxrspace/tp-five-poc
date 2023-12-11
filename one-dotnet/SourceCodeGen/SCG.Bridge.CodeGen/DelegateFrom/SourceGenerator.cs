using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TPFive.SCG.Bridge.CodeGen
{
    using TPFive.SCG.Bridge.Abstractions;
    using TPFive.SCG.Utility;

    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        public static string CrossGeneratedPath = Path.Combine(
            "core",
            "development",
            "common",
            "cross",
            "Runtime",
            "Scripts",
            "Generated");

        internal static string DefaultHandlerName = "SomeHandler";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
            {
                return;
            }

            // Not going to try catch as this is in the middle of compiling, no any resort to handle exception.
            foreach (var candidate in syntaxReceiver.Candidates)
            {
                var (fileName, sourceCode) =
                    GeneratePartialClass(candidate, context.Compilation);

                var sourceFilePath = candidate.Item1.SyntaxTree.FilePath;

                // SyntaxTree.FilePath will return empty string if the source is from a string. In such case,
                // most likely from testing, should just add the source code to the compilation.
                if (string.IsNullOrEmpty(sourceFilePath))
                {
                    context.AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));
                    continue;
                }

                var directoryInfo = new DirectoryInfo(sourceFilePath);
                var directoryPath = Path.GetDirectoryName(directoryInfo.FullName);
                var pathSegments = directoryPath.Split(Path.DirectorySeparatorChar);
                var lastIndex = Utility.GetAfterOneUnityPathIndex(directoryPath);

                if (lastIndex < 0)
                {
                    continue;
                }

                // Range operator is exclusive to the end so adding 1 back to make it inclusive.
                var rangeLastIndex = lastIndex + 1;
                var subPathSegments = pathSegments.Take(rangeLastIndex).ToArray();
                var commonPath = Path.Combine(subPathSegments);

                // On mac, this returns '/', on win, this returns "C:\"
                // So, only on mac this rootPath needs to be added.
                var rootPath = Path.GetPathRoot(directoryPath);
                var hasDriveRoot = commonPath.StartsWith(rootPath);
                var filePath = Path.Combine(commonPath, CrossGeneratedPath, fileName);
                if (!hasDriveRoot)
                {
                    filePath = $"{rootPath}{filePath}";
                }

                var fileMetaPath = Path.Combine(commonPath, CrossGeneratedPath, $"{fileName}.meta");
                if (!hasDriveRoot)
                {
                    fileMetaPath = $"{rootPath}{fileMetaPath}";
                }

                // Since the file is not presented, just create it.
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, sourceCode, Encoding.UTF8);
                    continue;
                }

                using var stream = File.OpenText(filePath);
                var fileContent = stream.ReadToEnd();

                // It is possible that the newline is different on different platform after checking out.
                // So stripping the newline before comparing.
                var pattern = @"[\n\r]";
                var strippedFileContent = Regex.Replace(fileContent, pattern, string.Empty);
                var strippedSourceCode = Regex.Replace(sourceCode, pattern, string.Empty);

                // Just make a simple string comparison to see if the file content is the same. Using
                // md5 or sha256 will introduce more complexity and slowing down compilation.
                var isEqual = strippedFileContent.Equals(strippedSourceCode, StringComparison.Ordinal);

                if (!isEqual)
                {
                    File.WriteAllText(filePath, sourceCode, Encoding.UTF8);
                }

                // Delete meta if corresponding file is not presented.
                if (!File.Exists(filePath))
                {
                    File.Delete(fileMetaPath);
                }
            }
        }

        private static (string FileName, string SourceCode) GeneratePartialClass(
            (ClassDeclarationSyntax, List<MethodDeclarationSyntax>) classMethodSyntax,
            Compilation compilation)
        {
            var (syntax, methodSyntaxList) = classMethodSyntax;
            var combinedMethodSyntaxList = new List<(MethodDeclarationSyntax, string)>();

            foreach (var methodSyntax in methodSyntaxList)
            {
                var methodSemanticModel = compilation.GetSemanticModel(methodSyntax.SyntaxTree);
                var methodSymbol = methodSemanticModel.GetDeclaredSymbol(methodSyntax);

                var methodHandler = GetHandlerValue(methodSymbol);

                combinedMethodSyntaxList.Add((methodSyntax, methodHandler));
            }

            var root = syntax.GetCompilationUnit();

            var properties = new List<PropertyData>();
            var classModel = new ClassModel(
                root.GetNamespace(),
                syntax.GetClassName(),
                syntax.GetClassModifier(),
                properties.ToArray());
            var source = CodeGenerator.Generate(classModel, combinedMethodSyntaxList);

            return ($"Bridge.{classModel.Namespace}.Generated.cs", source);
        }

        private static string GetHandlerValue(ISymbol classSymbol)
        {
            var delegateFromAttribute = classSymbol
                .GetAttributes()
                .FirstOrDefault(c => c.AttributeClass.Name == nameof(DelegateFromAttribute));
            var handler = delegateFromAttribute.GetAttributeValue(
                nameof(DelegateFromAttribute.DelegateName), DefaultHandlerName);

            return handler;
        }
    }
}
