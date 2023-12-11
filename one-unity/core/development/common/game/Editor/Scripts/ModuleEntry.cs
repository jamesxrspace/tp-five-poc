using System.Reactive;
using Autofac;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Serilog;
using Serilog.Sinks.Unity3D;
using Splat;
using Splat.Autofac;
using UnityEditor;
using UnityEngine;

namespace TPFive.Game.Editor
{
    using TPFive.Cross.Editor;

    using CrossBridge = TPFive.Cross.Editor.Bridge;

    /// <summary>
    /// This should be called according to order
    /// </summary>
    [OrderedInitializeOnLoad(0x0400)]
    public sealed partial class ModuleEntry
    {
        private static void OnLoadBegin(object someParams)
        {
            Debug.Log("[TPFive.Game.Editor.ModuleEntry] - OnLoadBegin");

            var exsitedLoggerFactory = Locator.Current.GetService<ILoggerFactory>();
            if (exsitedLoggerFactory == null)
            {
                Debug.Log("[TPFive.Game.Editor.ModuleEntry] - OnLoadBegin - Create LoggerFactory");

                var loggerConfig = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Unity3D(outputTemplate: "[{Level:u3}][{SourceContext}] {Message:j}{NewLine}{Exception}\n")
                    .WriteTo.Seq("http://localhost:5341");

                loggerConfig.MinimumLevel.Debug();

                var logger = loggerConfig.CreateLogger();
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddSerilog(logger);

                Locator.CurrentMutable
                    .Register<ILoggerFactory>(() => loggerFactory);
            }

            // // // TODO: Replace this with something else
            // // var lf = Locator.Current.GetService<ILoggerFactory>();
            // // if (lf != null) return;
            // //
            //
            // //
            // var loggerConfig = new LoggerConfiguration()
            //     .Enrich.FromLogContext()
            //     .WriteTo.Unity3D(outputTemplate: "[{Level:u3}][{SourceContext}] {Message:j}{NewLine}{Exception}\n")
            //     .WriteTo.Seq("http://localhost:5341");
            //
            // loggerConfig.MinimumLevel.Debug();
            //
            // var logger = loggerConfig.CreateLogger();
            // var loggerFactory = new LoggerFactory();
            // loggerFactory.AddSerilog(logger);
            //
            // Debug.Log("[TPFive.Game.Editor.ModuleEntry] - OnLoadBegin - Setup Container");
            // //
            // var builder = new ContainerBuilder();
            // var resolver = builder.UseAutofacDependencyResolver();
            //
            // builder.RegisterInstance(loggerFactory).As<ILoggerFactory>();
            //
            // //
            // builder.RegisterInstance(resolver);
            // resolver.InitializeSplat();
            // resolver.InitializeReactiveUI();
            //
            // // CrossBridge.BuildWithResolver = BuildWithResolver;
            //
            // CrossBridge.GetBuilderWithResolver = () => (builder, resolver);
            // //
            // // builder.RegisterInstance(new TPFive.Creator.Editor.DashboardViewModel());
            //
            // // //
            // // var container = builder.Build();
            // // resolver.SetLifetimeScope(container);
        }

        // private static void BuildWithResolver(object builderObj, object resolverObj)
        // {
        // }
        private static void OnLoadEnd(object someParams)
        {
            Debug.Log("[TPFive.Game.Editor.ModuleEntry] - OnLoadEnd");

#if XPOS_MAIN_PROJECT

            UnityEditor.Editor.finishedDefaultHeaderGUI -= HandleGameObjectHeader;
            UnityEditor.Editor.finishedDefaultHeaderGUI += HandleGameObjectHeader;

#endif

            //
            // var result = CrossBridge.GetBuilderWithResolver?.Invoke();
            //
            // if (result != null)
            // {
            //     var (builderObj, resolverObj) = result.Value;
            //     if (builderObj is ContainerBuilder builder
            //         && resolverObj is AutofacDependencyResolver resolver)
            //     {
            //         //
            //         var container = builder.Build();
            //         resolver.SetLifetimeScope(container);
            //     }
            // }
        }

        private static void HandleGameObjectHeader(UnityEditor.Editor editor)
        {
            if (editor.targets.Length > 1)
            {
                // Multi-object editing not supported
                return;
            }

            var gameObject = editor.target as GameObject;
            if (gameObject == null)
            {
                return;
            }

            // var componentBases = gameObject.GetComponents<ComponentBase>();
            // if (componentBases.Length == 0) return;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Game Scope");
            EditorGUILayout.EndVertical();
        }
    }
}
