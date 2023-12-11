using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TPFive.SCG.ServiceEco.CodeGen.InjectService
{
    internal static class CodeGenerator
    {
        public static string Generate(
            ClassModel model,
            string serviceList,
            string pubMessageList,
            string subMessageList,
            string addConstructor,
            string setup,
            string declareSettings,
            string addLifetimeScope,
            List<string> theList)
        {
            // Adjust to multi string literal won't be much clear, keep string builder usage.
            var sb = new StringBuilder();

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Cysharp.Threading.Tasks;");
            sb.AppendLine("using MessagePipe;");
            sb.AppendLine("using Microsoft.Extensions.Logging;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using VContainer;");
            sb.AppendLine("using VContainer.Unity;");
            sb.AppendLine("");
            sb.AppendLine($"namespace {model.Namespace}");
            sb.AppendLine("{");

            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine($"    {model.Modifier} class {model.Name}");
            sb.AppendLine("    {");
            sb.AppendLine(string.Empty);

            if (string.CompareOrdinal(addConstructor, "true") == 0)
            {
                sb.AppendLine("        private ILoggerFactory _loggerFactory;");
                sb.AppendLine(string.Empty);

                if (string.CompareOrdinal(addLifetimeScope, "true") == 0)
                {
                    sb.AppendLine("        private readonly LifetimeScope _lifetimeScope;");
                    sb.AppendLine(string.Empty);
                }

                if (string.CompareOrdinal(declareSettings, "true") == 0)
                {
                    sb.AppendLine("        private readonly Settings _settings;");
                    sb.AppendLine("        private readonly LifetimeScope.SceneSettings _sceneSettings;");
                    sb.AppendLine(string.Empty);
                }

                serviceList
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToList()
                    .ForEach(x =>
                    {
                        var category = x.Replace(".Service", "").Split('.').Last();
                        var serviceVarName = $"_service{category}";
                        var toInterface = x.Replace(".Service", ".IService");

                        sb.AppendLine($"        private readonly {toInterface} {serviceVarName};");
                    });
                sb.AppendLine(string.Empty);

                pubMessageList
                    .Split(',')
                    .Select(x => x)
                    .ToList()
                    .ForEach(x =>
                    {
                        var category = x.Split('.').Last();
                        var messageVarName = $"_pub{category}";

                        sb.AppendLine($"        private readonly IPublisher<{x}> {messageVarName};");
                    });
                sb.AppendLine(string.Empty);

                sb.AppendLine($"        public {model.Name}(");
                if (string.CompareOrdinal(declareSettings, "true") == 0)
                {
                    sb.AppendLine("            Settings settings,");
                    sb.AppendLine("            LifetimeScope.SceneSettings sceneSettings,");
                    sb.AppendLine(string.Empty);
                }

                var index = 0;
                var count = serviceList.Split(',').Length;
                serviceList
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToList()
                    .ForEach(x =>
                    {
                        ++index;
                        var category = x.Replace(".Service", "").Split('.').Last();
                        var serviceVarName = $"service{category}";
                        var toInterface = x.Replace(".Service", ".IService");
                        sb.AppendLine($"            {toInterface} {serviceVarName},");
                    });
                sb.AppendLine(string.Empty);

                index = 0;
                pubMessageList
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToList()
                    .ForEach(x =>
                    {
                        ++index;
                        var category = x.Split('.').Last();
                        var messageVarName = $"pub{category}";

                        sb.AppendLine($"            {x},");
                    });

                sb.AppendLine(string.Empty);

                if (string.CompareOrdinal(addLifetimeScope, "true") == 0)
                {
                    sb.AppendLine("            LifetimeScope lifetimeScope,");
                }

                sb.AppendLine("            ILoggerFactory loggerFactory");
                sb.AppendLine("            )");
                sb.AppendLine("        {");
                if (string.CompareOrdinal(declareSettings, "true") == 0)
                {
                    sb.AppendLine("            _settings = settings;");
                    sb.AppendLine("            _sceneSettings = sceneSettings;");
                    sb.AppendLine(string.Empty);
                }

                serviceList
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToList()
                    .ForEach(x =>
                    {
                        var category = x.Replace(".Service", "").Split('.').Last();
                        var serviceVarName = $"service{category}";
                        sb.AppendLine($"            _{serviceVarName} = {serviceVarName};");
                    });
                sb.AppendLine(string.Empty);

                if (string.CompareOrdinal(addLifetimeScope, "true") == 0)
                {
                    sb.AppendLine($"            _lifetimeScope = lifetimeScope; ");
                }

                sb.AppendLine(string.Empty);
                sb.AppendLine("            _loggerFactory = loggerFactory; ");
                sb.AppendLine("            Logger = loggerFactory.CreateLogger<Bootstrap>(); ");
                sb.AppendLine("        }");

                sb.AppendLine(string.Empty);

                sb.AppendLine("        private Microsoft.Extensions.Logging.ILogger Logger { get; }");
                sb.AppendLine(string.Empty);
            }

            sb.AppendLine("        private async UniTask SetupServices(CancellationToken cancellationToken = default)");
            sb.AppendLine("        {");
            sb.AppendLine("            var setupTasks = new List<UniTask>();");

            sb.AppendLine($"            // {theList.Count}");

            foreach (var cds in theList)
            {
                sb.AppendLine($"            // {cds}");
            }

            sb.AppendLine($"// declareSettings: {declareSettings}");
            sb.AppendLine($"// setup: {setup}");
            if (string.CompareOrdinal(setup, "true") == 0)
            {
                serviceList
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToList()
                    .ForEach(x =>
                    {
                        var category = x.Replace(".Service", "").Split('.').Last();
                        var serviceVarName = $"_service{category}";
                        sb.AppendLine($"            // Register Installer of {x}");
                        sb.AppendLine("            {");
                        sb.AppendLine($"                if ({serviceVarName} is IAsyncStartable asyncStartable)");
                        sb.AppendLine("                {");
                        sb.AppendLine("                    setupTasks.Add(asyncStartable.StartAsync(cancellationToken));");
                        sb.AppendLine("                }");
                        sb.AppendLine("            }");
                    });
                sb.AppendLine(string.Empty);
            }

            sb.AppendLine("            await UniTask.WhenAll(setupTasks);");

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            var output = sb.ToString();

            return output;
        }
    }
}
