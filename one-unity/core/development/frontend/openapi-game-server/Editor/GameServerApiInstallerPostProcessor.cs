using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using UnityEngine;
using XRSpace.OpenAPI;

namespace TPFive.OpenApi.GameServer
{
    /// <summary>
    /// For auto generate GameServerApiRegistry.cs.
    /// </summary>
    public class GameServerApiInstallerPostProcessor : IOpenApiCodegenPostProcessor
    {
        private const string Namespace = "TPFive.OpenApi.GameServer";
        private const string ClassName = "GameServerApiInstaller";

        private static readonly Regex ApiNameRegex = new Regex("(?<ApiName>[\\w]+)Api.cs");

        public string TargetName => "GameServer";

        public bool SingleProcess(string filePath, Action<Exception> onError)
        {
            return true;
        }

        public bool BatchProcess(string[] filePaths, Action<Exception> onError)
        {
            if (!PackageInfo.IsReady)
            {
                Debug.LogError("This package is not ready yet. Please try again.");
                return false;
            }

            var apiNames = GetApiNames(filePaths);
            if (!apiNames.Any())
            {
                // Skip generation if no api found.
                return true;
            }

            string outputDir = Path.Combine(PackageInfo.RootDir, "Runtime", "CodeGenerated");
            GenerateGameServerInstaller(apiNames, outputDir);

            return true;
        }

        private IEnumerable<string> GetApiNames(string[] filePaths)
        {
            var apiNames = new HashSet<string>();
            foreach (var filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);
                var match = ApiNameRegex.Match(fileName);
                if (match.Success)
                {
                    apiNames.Add(match.Groups["ApiName"].Value);
                }
            }

            return apiNames.OrderBy(name => name);
        }

        private void GenerateGameServerInstaller(IEnumerable<string> apiNames, string outputDir)
        {
            var compileUnit = new CodeCompileUnit();

            /*
             * namespace TPFive.OpenApi.GameServer
             * {
             *     using XRSpace.OpenAPI;
             *     using VContainer;
             *     using VContainer.Unity;
             *     ....
             */
            var ns = new CodeNamespace(Namespace);
            ns.Imports.Add(new CodeNamespaceImport("XRSpace.OpenAPI"));
            ns.Imports.Add(new CodeNamespaceImport("VContainer"));
            ns.Imports.Add(new CodeNamespaceImport("VContainer.Unity"));
            compileUnit.Namespaces.Add(ns);

            /*
             * public class GameServerApiInstaller : IInstaller
             * {
             *     private readonly Lifetime lifetime;
             *
             *     public GameServerApiInstaller(Lifetime lifetime)
             *     {
             *         this.lifetime = lifetime;
             *     }
             */
            var classDecl = new CodeTypeDeclaration(ClassName)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Class | TypeAttributes.Public,
            };
            classDecl.BaseTypes.Add(new CodeTypeReference("IInstaller"));
            ns.Types.Add(classDecl);

            // Generate 'lifetime' field
            CodeMemberField lifetimeField = new CodeMemberField("Lifetime", "lifetime");

            // Make that be a readonly field (dirty workaround)
            lifetimeField.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "\n        private readonly"));
            lifetimeField.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
            lifetimeField.Attributes = MemberAttributes.Final;
            classDecl.Members.Add(lifetimeField);

            // Generate constructor
            CodeConstructor constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
            constructor.Parameters.Add(new CodeParameterDeclarationExpression("Lifetime", "lifetime"));
            constructor.Statements.Add(new CodeSnippetStatement("            this.lifetime = lifetime;"));
            classDecl.Members.Add(constructor);

            // Generate RegisterApis method
            var registerApisMethod = GenerateInstallMethod(apiNames);
            classDecl.Members.Add(registerApisMethod);

            // Output
            string targetPath = Path.Combine(outputDir, $"{ClassName}.cs");
            using (StreamWriter writer = new StreamWriter(targetPath))
            {
                var provider = new CSharpCodeProvider();
                var options = new CodeGeneratorOptions() { BracingStyle = "C", };
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
                writer.Close();
            }
        }

        private CodeMemberMethod GenerateInstallMethod(IEnumerable<string> apiNames)
        {
            /*
             * public void Install(IContainerBuilder builder)
             * {
             *     // Register providers
             *     builder.Register<IServerBaseUriProvider, GameServerBaseUriProvider>(lifetime);
             *     builder.Register<IAuthTokenProvider, GameServerAuthTokenProvider>(lifetime);
             *     builder.Register<IErrorResponseResolver, GameServerErrorResponseResolver>(lifetime;
             *
             *     // Register Agora api
             *     builder.Register<I{apiName}Api, {apiName}Api>(lifetime);
             *     ...
             * }
             */
            var registerApisMethod = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = "Install",
                ReturnType = new CodeTypeReference(typeof(void)),
            };
            registerApisMethod.Parameters.Add(new CodeParameterDeclarationExpression("IContainerBuilder", "builder"));

            StringBuilder sb = new StringBuilder();

            // Register providers
            sb.AppendLine("            // Register providers");
            sb.AppendLine("            builder.Register<IServerBaseUriProvider, GameServerBaseUriProvider>(lifetime);");
            sb.AppendLine("            builder.Register<IAuthTokenProvider, GameServerAuthTokenProvider>(lifetime);");
            sb.AppendLine();

            // Register APIs
            foreach (string apiName in apiNames)
            {
                sb.AppendLine($"            // Register {apiName} api");
                sb.AppendLine($"            builder.Register<I{apiName}Api, {apiName}Api>(Lifetime.Scoped);");
            }

            CodeSnippetStatement registerStatement = new CodeSnippetStatement(sb.ToString());
            registerApisMethod.Statements.Add(registerStatement);

            return registerApisMethod;
        }
    }
}